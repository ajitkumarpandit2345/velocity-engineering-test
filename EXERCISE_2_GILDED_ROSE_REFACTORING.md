# Architecture Decision Record (ADR): Gilded Rose Inventory System Refactoring

**Document ID:** `Exercise 2: Gilded Rose Refactoring Kata`  
**Author:** Lead Fullstack Engineer  
**Status:** Accepted & Implemented  
**Target:** `GildedRose.Console` & `GildedRose.Tests`  

---

## 1. Context & Problem Statement

The Gilded Rose inventory system manages goods whose `SellIn` and `Quality` metrics update on a daily cadence. The legacy codebase suffered from technical debt:

1. **High Cyclomatic Complexity:** All inventory rules were packed into a single, monolithic `UpdateQuality()` loop containing deeply nested `if/else` ladders (up to 4 levels deep).
2. **SRP Violation:** The `Program` class acted as the application entry point, data holder, and business rule engine simultaneously.
3. **Fragile Modification Surface:** Introducing the new **Conjured items** rule via additional boolean flags in the legacy loop would worsen readability and risk introducing regressions to established products.
4. **The Goblin Constraint:** The specification strictly forbids modifying the `Item` class or its properties (`Name`, `SellIn`, `Quality`). We cannot introduce polymorphism directly on `Item` (e.g., `class AgedBrie : Item`).

---

## 2. Decision: Strategy Pattern with Dynamic Resolution

### Alternatives Considered

| Approach | Pros | Cons | Decision |
|:---|:---|:---|:---|
| **Keep Single Method + Helpers** | Simple; low file count. | Violates Open/Closed Principle. Adding future supplier items requires modifying shared degradation logic. | **Rejected** |
| **Inheritance on `Item`** | Idiomatic OOP. | Violates specification constraint (goblin owns `Item`). | **Rejected** |
| **Strategy Pattern (`IItemUpdateStrategy`)** | Strict OCP/SRP compliance. Adding a supplier item requires creating 1 isolated class with zero risk to existing items. | Slightly more initial boilerplate. | **Accepted** ✅ |

### The Open/Closed Principle in Practice
A critical architectural consideration raised during design was:
> *"If we write a single method for both Standard and Conjured items, what happens when a new supplier introduces another item category?"*

Bundling them together creates a hidden coupling point. By implementing the **Strategy Pattern**, each item type encapsulates its own lifecycle rules:
- Standard goods, Aged Brie, Backstage Passes, Sulfuras, and Conjured items have zero code dependencies on each other.
- When new suppliers or item categories arrive, engineers only implement a new `IItemUpdateStrategy` and register it in `ItemStrategyFactory`. No existing logic is touched.

---

## 3. Architecture & Component Design

```
                     ┌───────────────────────┐
                     │      Program.cs       │  (CLI Entry Point)
                     └──────────┬────────────┘
                                │ delegates
                                ▼
                     ┌───────────────────────┐
                     │   GildedRoseService   │  (Domain Orchestrator)
                     └──────────┬────────────┘
                                │
                                ▼
                     ┌───────────────────────┐
                     │  ItemStrategyFactory  │  (Strategy Resolver)
                     └──────────┬────────────┘
                                │ resolves
                                ▼
                     ┌───────────────────────┐
                     │ <<IItemUpdateStrategy │
                     └──────────┬────────────┘
                                │
       ┌────────────────────────┼────────────────────────┬─────────────────────┐
       ▼                        ▼                        ▼                     ▼
┌──────────────┐         ┌──────────────┐         ┌─────────────┐       ┌──────────────┐
│ StandardItem │         │   AgedBrie   │         │  Backstage  │       │   Conjured   │
│   Strategy   │         │   Strategy   │         │PassStrategy │       │ ItemStrategy │
└──────────────┘         └──────────────┘         └─────────────┘       └──────────────┘
```

### Component Roles

1. **[`IItemUpdateStrategy.cs`](file:///d:/0-Tech%20Interviews/Antigravity/velocity-engineering-test/src/GildedRose.Console/Strategies/IItemUpdateStrategy.cs)**  
   Defines the contract:
   - `bool CanHandle(Item item)`: Evaluates whether the strategy applies to the given item.
   - `void Update(Item item)`: Executes daily degradation/appreciation and expiration logic.

2. **Isolated Strategy Implementations:**
   - **`StandardItemStrategy`**: Normal decay ($-1$/day, $-2$ post-expiry, floor $0$). Acts as default fallback.
   - **`ConjuredItemStrategy`**: Fast decay ($-2$/day, $-4$ post-expiry, floor $0$). Matches `Name.StartsWith("Conjured")`.
   - **`AgedBrieStrategy`**: Appreciation ($+1$/day, $+2$ post-expiry, ceiling $50$).
   - **`BackstagePassStrategy`**: Tiered appreciation ($+1$ standard, $+2$ at $\le 10$ days, $+3$ at $\le 5$ days, drops to $0$ post-concert).
   - **`SulfurasStrategy`**: Immutable legendary item; skips changes entirely.

3. **[`ItemStrategyFactory.cs`](file:///d:/0-Tech%20Interviews/Antigravity/velocity-engineering-test/src/GildedRose.Console/Strategies/ItemStrategyFactory.cs)**  
   Resolves the appropriate strategy using an ordered registry. If no specific rule matches, it seamlessly defaults to `StandardItemStrategy`.

4. **[`GildedRoseService.cs`](file:///d:/0-Tech%20Interviews/Antigravity/velocity-engineering-test/src/GildedRose.Console/GildedRoseService.cs)**  
   Separates domain execution from the CLI console, allowing the inventory logic to be consumed by APIs, background jobs, or test harnesses with equal ease.

---

## 4. Verification & Quality Assurance

A regression safety net of **16 xUnit automated tests** was established in [`TestAssemblyTests.cs`](file:///d:/0-Tech%20Interviews/Antigravity/velocity-engineering-test/src/GildedRose.Tests/TestAssemblyTests.cs). 

All standard items, boundary conditions, edge cases ($SellIn = 0$, $Quality = 50$, $Quality = 0$, $Quality = 80$), backstage pass tiers, and new conjured degradation mechanics pass cleanly:

```text
Build succeeded. (0 Warnings, 0 Errors)
Total tests: 16 | Passed: 16 | Failed: 0 | Skipped: 0
```

---

## 5. How to Add a New Item Category in the Future

1. Create a new class implementing `IItemUpdateStrategy` (e.g., `PerishableItemStrategy.cs`).
2. Implement `CanHandle(Item item)` and `Update(Item item)`.
3. Add the new strategy instance to `ItemStrategyFactory.SpecificStrategies`.
4. Add corresponding xUnit test cases.

Existing item code remains 100% untouched.
