using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using SharpGen.Doc;
using SharpGenTools.Sdk.Internal;

namespace SharpGenTools.Sdk.Documentation
{
    public sealed class DocItem : IDocItem
    {
        private readonly ObservableCollection<IDocSubItem> items = new ObservableCollection<IDocSubItem>();

        private readonly ObservableHashSet<string> names =
            new ObservableHashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private readonly ObservableHashSet<string> seeAlso =
            new ObservableHashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private bool isDirty;
        private string remarks;
        private string @return;
        private string shortId;
        private string summary;

        public DocItem()
        {
            names.CollectionChanged += OnCollectionChanged;
            items.CollectionChanged += OnCollectionChanged;
            seeAlso.CollectionChanged += OnCollectionChanged;
        }

        public string ShortId
        {
            get => shortId;
            set
            {
                if (shortId == value) return;

                shortId = value;
                IsDirty = true;
            }
        }

        public ISet<string> Names => names;

        public string Summary
        {
            get => summary;
            set
            {
                if (summary == value) return;

                summary = value;
                IsDirty = true;
            }
        }

        public string Remarks
        {
            get => remarks;
            set
            {
                if (remarks == value) return;

                remarks = value;
                IsDirty = true;
            }
        }

        public string Return
        {
            get => @return;
            set
            {
                if (@return == value) return;

                @return = value;
                IsDirty = true;
            }
        }

        public IList<IDocSubItem> Items => items;

        public ISet<string> SeeAlso => seeAlso;

        public bool IsDirty
        {
            get => isDirty ? isDirty : isDirty = items.Any(DirtyPredicate);
            set => isDirty = value;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        private static bool DirtyPredicate(IDocSubItem x)
        {
            return x.IsDirty;
        }
    }
}