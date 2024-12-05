using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree;

public class DiskTreeTask
{
    public static List<string> Solve(List<string> input)
    {
        var catalogTree = new CatalogTree();
        foreach (var directoryPath in input)
            catalogTree.Add(directoryPath);
        return catalogTree.ToString().Split('\n').ToList();
    }
}

class CatalogTree
{
    private class Catalog
    {
        public readonly string Name;
        public readonly int Level;
        public List<Catalog> SubCatalogs { get; set; }

        public Catalog(string name, int level)
        {
            Name = name;
            Level = level;
            SubCatalogs = new List<Catalog>();
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not Catalog)
                return false;
            var other = (Catalog)obj;
            return Name == other.Name && Level == other.Level;
        }
    }

    private readonly Catalog rootCatalog = new("Root", 0);

    public void Add(string directoryPath)
    {
        var directoryParts = directoryPath.Split('\\');
        var currentCatalog = rootCatalog;
        var currentLevel = 1;
        foreach (var part in directoryParts)
        {
            var catalog = new Catalog(part, currentLevel);
            if (!currentCatalog.SubCatalogs.Contains(catalog))
                currentCatalog.SubCatalogs.Add(catalog);
            var catalogIndex = currentCatalog.SubCatalogs.IndexOf(catalog);
            currentCatalog = currentCatalog.SubCatalogs[catalogIndex];
            currentLevel++;
        }
    }

    public override string ToString()
    {
        var lines = new List<string>();
        var stack = new Stack<Catalog>();
        stack.Push(rootCatalog);
        while (stack.Count > 0)
        {
            var currentCatalog = stack.Pop();
            if (!Equals(currentCatalog, rootCatalog))
                lines.Add(new string(' ', currentCatalog.Level - 1) + currentCatalog.Name);
            foreach (var subCatalog in
                     currentCatalog.SubCatalogs.OrderByDescending(x => x.Name, StringComparer.Ordinal))
            {
                stack.Push(subCatalog);
            }
        }
        return string.Join('\n', lines);
    }
}