﻿using System.Text.RegularExpressions;
using BlazorDatasheet.DataStructures.Util;

namespace BlazorDatasheet.Formula.Core.Interpreter.References;

public class CellReference : Reference
{
    private readonly ColReference _col;
    private readonly RowReference _row;
    public ColReference Col => _col;
    public RowReference Row => _row;

    public CellReference(int row, int col, bool absoluteCol = false, bool absoluteRow = false) : this(
        new RowReference(row, absoluteRow), new ColReference(col, absoluteCol))
    {
    }

    public CellReference(RowReference row, ColReference col)
    {
        _row = row;
        _col = col;
    }

    public override ReferenceKind Kind { get; }

    public override string ToRefText()
    {
        return $"{Col.ToRefText()}{Row.ToRefText()}";
    }

    public override bool SameAs(Reference reference)
    {
        if (reference.Kind == ReferenceKind.Cell)
        {
            var cellRef = (CellReference)reference;
            return Col.SameAs(cellRef.Col) &&
                   Row.SameAs(cellRef.Row);
        }

        if (reference.Kind == ReferenceKind.Range)
        {
            var rangeRef = (RangeReference)reference;
            return this.SameAs(rangeRef.Start) &&
                   this.SameAs(rangeRef.End);
        }

        return false;
    }

    public override void Shift(int offsetRow, int offsetCol)
    {
        _col.Shift(offsetRow, offsetCol);
        _row.Shift(offsetRow, offsetCol);
    }

    public override string ToString() => ToRefText();
}