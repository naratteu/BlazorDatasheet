using System.Linq;
using BlazorDatasheet.DataStructures.Geometry;
using BlazorDatasheet.DataStructures.Store;
using FluentAssertions;
using NUnit.Framework;

namespace BlazorDatasheet.Test.Store;

public class DataStoreTests
{
    [Test]
    public void Set_Get_Operations_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        Assert.AreEqual(default(string), store.Get(0, 0));
        store.Set(0, 0, "A");
        Assert.AreEqual("A", store.Get(0, 0));
        store.Set(10, 10, "B");
        Assert.AreEqual("B", store.Get(10, 10));
        store.Set(10000000, 10000000, "C");
        Assert.AreEqual("C", store.Get(10000000, 10000000));
    }

    [Test]
    public void Insert_Row_At_Existing_Operations_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "A");
        store.Set(1, 0, "B");
        store.Set(2, 0, "C");
        store.Set(3, 0, "D");
        store.InsertRowAt(0, 1);
        Assert.AreEqual(default(string), store.Get(0, 0));
        Assert.AreEqual("A", store.Get(1, 0));
        Assert.AreEqual("B", store.Get(2, 0));
        Assert.AreEqual("C", store.Get(3, 0));
        Assert.AreEqual("D", store.Get(4, 0));
        Assert.AreEqual(default(string), store.Get(5, 0));
    }

    [Test]
    public void Insert_2_Rows_At_existing_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "A");
        store.Set(1, 0, "B");
        store.Set(2, 0, "C");
        store.Set(3, 0, "D");
        store.InsertRowAt(1, 2);
        Assert.AreEqual("A", store.Get(0, 0));
        Assert.AreEqual(default(string), store.Get(1, 0));
        Assert.AreEqual(default(string), store.Get(2, 0));
        Assert.AreEqual("B", store.Get(3, 0));
        Assert.AreEqual("C", store.Get(4, 0));
        Assert.AreEqual("D", store.Get(5, 0));
        Assert.AreEqual(default(string), store.Get(6, 0));
    }

    [Test]
    public void Insert_Row_After_Non_Existing_Operations_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "A");
        store.Set(2, 0, "B");
        store.Set(3, 0, "C");
        store.InsertRowAt(1, 1);
        Assert.AreEqual("A", store.Get(0, 0));
        Assert.AreEqual(default(string), store.Get(1, 0));
        Assert.AreEqual(default(string), store.Get(2, 0));
        Assert.AreEqual("B", store.Get(3, 0));
        Assert.AreEqual("C", store.Get(4, 0));
    }

    [Test]
    public void Remove_NonEmpty_Row_Operations_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "A");
        store.Set(1, 0, "B");
        store.Set(2, 0, "C");
        store.Set(3, 0, "D");
        store.RemoveRowAt(0, 1);
        Assert.AreEqual("B", store.Get(0, 0));
        Assert.AreEqual("C", store.Get(1, 0));
        Assert.AreEqual("D", store.Get(2, 0));
        Assert.AreEqual(default(string), store.Get(3, 0));
    }

    [Test]
    public void Remove_Two_Rows_Removes_Both()
    {
        IMatrixDataStore<int> store = new SparseMatrixStore2<int>();
        store.Set(0, 0, 0);
        store.Set(1, 0, 1);
        store.Set(2, 0, 2);
        store.Set(3, 0, 3);
        store.RemoveRowAt(1, 2);
        store.Get(0, 0).Should().Be(0);
        store.Get(1, 0).Should().Be(3);
        store.Get(2, 0).Should().Be(default(int));
    }

    [Test]
    public void Remove_Empty_Row_Operations_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "A");
        store.Set(2, 0, "B");
        store.Set(3, 0, "C");
        store.RemoveRowAt(1, 1);
        Assert.AreEqual("A", store.Get(0, 0));
        Assert.AreEqual("B", store.Get(1, 0));
        Assert.AreEqual("C", store.Get(2, 0));
    }

    [Test]
    public void Get_Next_Non_Empty_Row_Num_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(5, 10, "A");
        store.Set(100, 10, "B");
        var nextRow = store.GetNextNonBlankRow(5, 10);
        Assert.AreEqual(100, nextRow);
        Assert.AreEqual("B", store.Get(nextRow, 10));
    }

    [Test]
    public void Get_Non_Empty_Rows_Correct()
    {
        IMatrixDataStore<string> store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "0,0");
        store.Set(2, 0, "0,0");
        var nonEmpty = store.GetNonEmptyPositions(0, 2, 0, 0);
        Assert.AreEqual(2, nonEmpty.Count());
    }

    [Test]
    public void Clear_Cell_Clears_Cell_Correctly()
    {
        var store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "0,0");
        store.Set(0, 1, "0,1");
        store.Set(1, 0, "1,0");
        store.Clear(0, 0);
        Assert.Null(store.Get(0, 0));
        Assert.AreEqual("0,1", store.Get(0, 1));
        Assert.AreEqual("1,0", store.Get(1, 0));
    }

    [Test]
    public void Clear_Cells_Removes_Cells_From_Store()
    {
        var store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "'0,0");
        store.Set(0, 1, "'0,1");
        store.Set(1, 0, "'1,0");
        store.GetNonEmptyData(new Region(0, 1, 0, 1)).Count().Should().Be(3);
        store.Clear(new Region(0, 1, 0, 1));
        store.GetNonEmptyData(new Region(0, 1, 0, 1)).Count().Should().Be(0);
    }

    [Test]
    public void Remove_Region_Removes_Region()
    {
        var store = new SparseMatrixStore2<string>();
        store.Set(0, 0, "0,0");
        store.Set(0, 1, "0,1");
        store.Set(1, 0, "1,0");
        store.Set(1, 1, "1,1");
        store.Set(2, 2, "2,2");
        store.Clear(new Region(0, 1, 0, 1));
        store.Get(0, 0).Should().BeNullOrEmpty();
        store.Get(0, 1).Should().BeNullOrEmpty();
        store.Get(1, 0).Should().BeNullOrEmpty();
        store.Get(1, 1).Should().BeNullOrEmpty();
        store.Get(2, 2).Should().Be("2,2");
    }
}