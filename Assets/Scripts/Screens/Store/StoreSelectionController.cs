public class StoreSelectionController
{
    private StoreItemView selectedView;

    public bool HasSelection => selectedView != null;

    public bool Select(string itemId)
    {
        if (selectedView != null && selectedView.Data.id == itemId)
            return false;

        return true; // New selection
    }

    public void UpdateSelection(StoreItemView newlySelected)
    {
        // Deselect the previous one
        if (selectedView != null)
            selectedView.SetSelected(false);

        selectedView = newlySelected;
        selectedView.SetSelected(true);
    }

    public void Clear()
    {
        if (selectedView != null)
            selectedView.SetSelected(false);

        selectedView = null;
    }
}