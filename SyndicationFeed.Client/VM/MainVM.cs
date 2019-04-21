using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SyndicationFeed.Client.VM
{
    class MainVM : VM
    {
        public MainVM(UserVM user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            user.PropertyChanged += OnUserPropertyChanged;
            AddCollectionCommand = new SimpleCommand(OnAddCollection);
            RemoveCurrentCollectionCommand = new SimpleCommand(OnRemoveCurrentCollection) { AllowExecute = false };
        }

        async void OnUserPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (name == nameof(User.IsAuthenticated))
            {
                bool isAuthenticated = User.IsAuthenticated;
                AddCollectionCommand.AllowExecute = isAuthenticated;
                if (isAuthenticated)
                    await TryShowCollections();
                else
                    HideCollections();
            }
        }

        async Task TryShowCollections()
        {
            if (!User.IsAuthenticated)
                return;

            Status = "Loading collections...";
            try
            {
                await LoadCollections();
                Status = "Collections loaded";
            }
            catch (Exception ex)
            {
                // TODO: differentiate exception types
                Status = "Error loading collections";
            }
        }

        async Task LoadCollections()
        {
            if (!User.IsAuthenticated)
                throw new InvalidOperationException("Must be authenticated to access the collections");

            Collections = null;
            CurrentCollection = null;
            var root = User.GetFeedRoot();
            var modelCollections = await root.GetAllCollections();
            Collections = modelCollections.Select(coll => new CollectionVM(coll)).ToList();
        }

        void HideCollections()
        {
            CurrentCollection = null;
            Collections = null;
        }

        public UserVM User { get; }

        public SimpleCommand AddCollectionCommand { get; }
        public SimpleCommand RemoveCurrentCollectionCommand { get; }

        string status;
        public string Status
        {
            get => status;
            private set => Set(ref status, value);
        }

        List<CollectionVM> collections;
        public List<CollectionVM> Collections
        {
            get => collections;
            private set
            {
                if (Set(ref collections, value))
                    CollectionCount = collections?.Count ?? 0;
            }
        }

        int collectionCount;
        public int CollectionCount
        {
            get => collectionCount;
            private set => Set(ref collectionCount, value);
        }

        CollectionVM currentCollection;
        public CollectionVM CurrentCollection
        {
            get => currentCollection;
            set
            {
                if (Set(ref currentCollection, value))
                {
                    RemoveCurrentCollectionCommand.AllowExecute = (currentCollection != null);
                    if (currentCollection != null)
                    {
                        _ = currentCollection.LoadFeeds();
                    }
                }
            }
        }

        AddCollectionVM addCollection;
        public AddCollectionVM AddCollection
        {
            get => addCollection;
            set => Set(ref addCollection, value);
        }

        async void OnAddCollection()
        {
            if (!User.IsAuthenticated)
                return;

            try
            {
                AddCollectionCommand.AllowExecute = false;
                var root = User.GetFeedRoot();
                AddCollection = new AddCollectionVM(root);
                long? id = await AddCollection.Execution;
                if (id != null)
                {
                    await TryShowCollections();
                    if (Collections != null)
                        CurrentCollection = Collections.FirstOrDefault(coll => coll.Id == id);
                }
            }
            finally
            {
                AddCollection = null;
                AddCollectionCommand.AllowExecute = User.IsAuthenticated;
            }
        }

        async void OnRemoveCurrentCollection()
        {
            if (!User.IsAuthenticated || CurrentCollection == null)
                return;

            try
            {
                RemoveCurrentCollectionCommand.AllowExecute = false;
                var root = User.GetFeedRoot();
                await CurrentCollection.RemoveItself(root);
                CurrentCollection = null;
                await TryShowCollections();
            }
            finally
            {
                RemoveCurrentCollectionCommand.AllowExecute = (CurrentCollection != null);
            }
        }
    }
}
