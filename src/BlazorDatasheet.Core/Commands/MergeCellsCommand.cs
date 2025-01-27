using BlazorDatasheet.Core.Data;
using BlazorDatasheet.Core.Data.Cells;
using BlazorDatasheet.DataStructures.Geometry;
using BlazorDatasheet.Formula.Core;

namespace BlazorDatasheet.Core.Commands;

public class MergeCellsCommand : IUndoableCommand
{
    private readonly List<IRegion> _overridenMergedRegions = new();
    private readonly List<IRegion> _mergesPerformed = new();
    private CellStoreRestoreData _restoreData;
    private IRegion _region;

    /// <summary>
    /// Command that merges the cells in the range give.
    /// Note that the value in the top LHS will be kept, while other cell values will be cleared.
    /// </summary>
    /// <param name="range">The range in which to merge. </param>
    public MergeCellsCommand(IRegion region)
    {
        _region = region.Clone();
    }


    public bool Execute(Sheet sheet)
    {
        _overridenMergedRegions.Clear();
        _mergesPerformed.Clear();

        var region = _region;
        // Determine if there are any merged cells in the region
        // We can only merge over merged cells if we entirely overlap them
        var existingMerges = sheet.Cells.GetMerges(region).ToList();
        if (!existingMerges.All(x => region.Contains(x)))
            return false;

        // Clear all the cells that are not the top-left posn of merge and store their values for undo
        var regionsToClear = region
            .Break(region.TopLeft)
            .ToList();

        _restoreData = sheet.Cells.ClearCellsImpl(regionsToClear);

        // Store the merges that we will have to re-instate on undo
        // And remove any merges that are contained in the region
        _overridenMergedRegions.AddRange(existingMerges);
        foreach (var existingMerge in existingMerges)
        {
            sheet.Cells.UnMergeCellsImpl(new SheetRange(sheet, existingMerge));
        }

        // Store the merge that we are doing and perform the actual merge
        _mergesPerformed.Add(region);
        sheet.Cells.MergeImpl(region);
        return true;
    }

    private CellValueChange getValueChangeOnClear(int row, int col, Sheet sheet)
    {
        return new CellValueChange(row, col, sheet.Cells.GetValue(row, col));
    }

    public bool Undo(Sheet sheet)
    {
        // Undo the merge we performed
        foreach (var merge in _mergesPerformed)
            sheet.Cells.UnMergeCellsImpl(merge);
        // Restore all the merges we removed
        foreach (var removedMerge in _overridenMergedRegions)
            sheet.Cells.MergeImpl(removedMerge);

        // Restore all the cell values that were lost when merging
        sheet.Cells.Restore(_restoreData);

        return true;
    }
}