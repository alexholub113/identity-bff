# MobX Store Architecture Guide

This project uses MobX for state management with a clean, scalable store architecture. Here's how to work with and extend the store system.

## Store Structure

```
src/stores/
├── AuthStore.ts        # Authentication state and actions
├── DataStore.ts        # Protected data management
├── RootStore.ts        # Combines all stores
├── StoreContext.tsx    # React context provider
└── index.ts           # Exports all store-related items
```

## How to Add a New Store

### 1. Create the Store Class

```typescript
// src/stores/ExampleStore.ts
import { makeAutoObservable, runInAction } from 'mobx';

export class ExampleStore {
    items: string[] = [];
    isLoading = false;
    error = '';

    constructor() {
        makeAutoObservable(this);
    }

    // Computed values (derived state)
    get itemCount() {
        return this.items.length;
    }

    // Actions (methods that modify state)
    setLoading(loading: boolean) {
        this.isLoading = loading;
    }

    setError(error: string) {
        this.error = error;
    }

    addItem(item: string) {
        this.items.push(item);
    }

    // Async actions
    async fetchItems() {
        this.setLoading(true);
        this.setError('');
        
        try {
            // Simulate API call
            await new Promise(resolve => setTimeout(resolve, 1000));
            
            runInAction(() => {
                this.items = ['Item 1', 'Item 2', 'Item 3'];
            });
        } catch (error) {
            runInAction(() => {
                this.setError('Failed to fetch items');
            });
        } finally {
            runInAction(() => {
                this.setLoading(false);
            });
        }
    }
}
```

### 2. Add Store to RootStore

```typescript
// src/stores/RootStore.ts
import { AuthStore } from './AuthStore';
import { DataStore } from './DataStore';
import { ExampleStore } from './ExampleStore';

export class RootStore {
    authStore: AuthStore;
    dataStore: DataStore;
    exampleStore: ExampleStore;

    constructor() {
        this.authStore = new AuthStore();
        this.dataStore = new DataStore();
        this.exampleStore = new ExampleStore();
    }
}
```

### 3. Add Hook to StoreContext

```typescript
// src/stores/StoreContext.tsx
export const useExampleStore = () => useStore().exampleStore;
```

### 4. Export from Index

```typescript
// src/stores/index.ts
export { ExampleStore } from './ExampleStore';
export { useExampleStore } from './StoreContext';
```

### 5. Use in Components

```typescript
// src/components/ExampleComponent.tsx
import React from 'react';
import { observer } from 'mobx-react-lite';
import { useExampleStore } from '../stores';

const ExampleComponent = observer(() => {
    const exampleStore = useExampleStore();

    return (
        <div>
            <h3>Items ({exampleStore.itemCount})</h3>
            
            <button 
                onClick={() => exampleStore.fetchItems()}
                disabled={exampleStore.isLoading}
            >
                {exampleStore.isLoading ? 'Loading...' : 'Fetch Items'}
            </button>

            {exampleStore.error && (
                <p style={{ color: 'red' }}>{exampleStore.error}</p>
            )}

            <ul>
                {exampleStore.items.map((item, index) => (
                    <li key={index}>{item}</li>
                ))}
            </ul>
        </div>
    );
});

export default ExampleComponent;
```

## Best Practices

### 1. Always Use `makeAutoObservable()`
This automatically makes all properties observable and all methods actions.

### 2. Use `runInAction()` for Async Updates
When updating state after async operations, wrap the updates in `runInAction()`.

### 3. Separate Actions from UI Logic
Keep business logic in stores, not in components. Components should only call actions and display state.

### 4. Use `observer()` for Components
Any component that reads MobX state should be wrapped with `observer()` to react to changes.

### 5. Computed Values for Derived State
Use getters for computed values that depend on observable state.

### 6. Error Handling
Always include error handling in async actions and store error state in the store.

## Benefits of This Architecture

1. **Reactive Updates**: UI automatically updates when state changes
2. **Centralized State**: All application state is in stores
3. **Type Safety**: Full TypeScript support
4. **Predictable Updates**: All state changes go through actions
5. **Easy Testing**: Stores can be tested independently
6. **Scalable**: Easy to add new stores as the app grows
7. **DevTools**: Excellent debugging with MobX DevTools

## Debugging

Install MobX DevTools browser extension for excellent debugging capabilities:
- View store state in real-time
- Track state changes and actions
- Time-travel debugging
- Performance monitoring
