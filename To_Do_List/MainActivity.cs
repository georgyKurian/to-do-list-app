using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Views;
using Android.Content;
using Newtonsoft.Json;

namespace To_Do_List
{
    [Activity(Label = "To Do List", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private ListView toDoListView;
        private ProgressBar progressBar;

        private List<ToDo> toDoListData;
        private ToDoListViewAdapter toDoListAdapter;
        

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.ToDoList);

            toDoListView = FindViewById<ListView>(Resource.Id.toDoListView);
            toDoListView.ItemClick += ToDoClicked;

            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);

            
        }

        protected override void OnResume()
        {
            base.OnResume();

            DownloadToDoListAsync();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            // Inflate menus to be shown on ActionBar
            MenuInflater.Inflate(Resource.Menu.ListtMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.actionNew:
                    // place holder for creating new poi
                    StartActivity(typeof(ToDoDetailActivity));
                    return true;
                case Resource.Id.actionRefresh:
                    DownloadToDoListAsync();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
        public async void DownloadToDoListAsync()
        {
            ToDoServices service = new ToDoServices();
            if (!service.isConnected(this))
            {
                Toast toast = Toast.MakeText(this, "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
            }
            else
            {
                progressBar.Visibility = ViewStates.Visible;
                toDoListData = await service.GetToDoListAsync();
                progressBar.Visibility = ViewStates.Gone;

                toDoListAdapter = new ToDoListViewAdapter(this, toDoListData);
                toDoListView.Adapter = toDoListAdapter;
            }
        }

        /**
		 * Delegate that handles POI ListView row click event.
		 **/
        protected void ToDoClicked(object sender, AdapterView.ItemClickEventArgs e) { 
        
            
            ToDo toDo = toDoListData[(int)e.Id];

            Intent toDoDetailIntent = new Intent(this, typeof(ToDoDetailActivity));
            string toDoJson = JsonConvert.SerializeObject(toDo);
            toDoDetailIntent.PutExtra("toDo", toDoJson);
            StartActivity(toDoDetailIntent);
            
        }

    }
}

