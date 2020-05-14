using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SharpGen.Doc;
using SharpGenTools.Sdk.Internal;

namespace SharpGenTools.Sdk.Documentation
{
    /// <inheritdoc />
    public sealed class DocSubItem : IDocSubItem
    {
        private readonly ObservableHashSet<string> attributes =
            new ObservableHashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private string description;
        private string term;

        public DocSubItem()
        {
            attributes.CollectionChanged += OnCollectionChanged;
        }

        public string Term
        {
            get => term;
            set
            {
                if (term == value) return;

                term = value;
                IsDirty = true;
            }
        }

        public string Description
        {
            get => description;
            set
            {
                if (description == value) return;

                description = value;
                IsDirty = true;
            }
        }

        public ISet<string> Attributes => attributes;

        public bool IsDirty { get; set; }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }
    }
}