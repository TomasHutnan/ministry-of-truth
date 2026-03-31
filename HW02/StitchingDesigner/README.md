# Stitching Designer

A .NET MAUI application for designing cross-stitch patterns using the DMC floss color palette.

## Usage

### 1. Setting Grid Size
- Enter the desired number of **Rows** and **Columns** in the "Grid Size" section
- Click **Set** to create a new grid
- Your pattern will be copied to the new grid, any out of bounds cells will be ignored

### 2. Selecting a Color
- Use the **Search** bar to find floss by ID or name (e.g., "3713", "Salmon")
- Click any color in the list to select it
- The selected color appears in the "Currently Selected" button with its preview

### 3. Drawing the Pattern
- Click any square in the grid to fill it with the currently selected color
- The floss ID is displayed in each filled square

### 4. Saving Your Pattern
- Enter a **Pattern Name** (e.g., "my_flower", "star")
- Click **Save** to store your pattern
- The file path is displayed below the button for reference

### 5. Loading a Pattern
- Enter the pattern name you want to load
- Click **Load** to restore the pattern to the grid
- You can continue editing loaded patterns

## Pattern File Format

Patterns are saved as JSON files (e.g., `my_flower.json`):

```json
{
  "rowCount": 10,
  "columnCount": 15,
  "cells": [
    {"flossId": "3713", "row": 0, "col": 5},
    {"flossId": "761", "row": 1, "col": 3},
    {"flossId": "760", "row": 2, "col": 4}
  ]
}
```

- Only **filled** cells are stored
- Each cell records: floss ID, row, and column position
