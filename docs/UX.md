# UI/UX Notes

## Validation visuals
- Text boxes now display red borders and tooltips when validation fails using a shared template.
- Dialogs display inline error text bound to `Validation.Errors` for immediate feedback.

## Data grid rows
- Rows alternate background colors in the light theme.
- Selected rows highlight with a light blue shade for clarity.

## Supplier selector
- Type a supplier name in the combo box and press Enter to confirm.
- If the name does not exist, the selector offers to create it via a dialog.
- Use the Add button to open the creation dialog directly.

## Screen states
- Browsing mode shows the invoice list and disables detail inputs.
- Editing mode is activated when creating or opening an invoice and enables the detail pane.
- Save and Cancel commands switch back to Browsing mode.
