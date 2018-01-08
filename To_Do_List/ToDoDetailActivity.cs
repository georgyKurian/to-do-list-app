using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace To_Do_List
{
    [Activity(Label = "To Do Detail")]
    public class ToDoDetailActivity : Activity
    {
        private ToDo _toDo;

        private EditText _editHeading;
        private EditText _editDescription;
        private CheckBox _editIsComplete;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.TaskDetails);

            _editHeading = FindViewById<EditText>(Resource.Id.taskHeading);
            _editDescription = FindViewById<EditText>(Resource.Id.taskDescription);
            _editIsComplete = FindViewById<CheckBox>(Resource.Id.taskIsComplete);

            // Retriving the bundle extras available with intent
            if (Intent.HasExtra("toDo"))
            {
                string poiJson = Intent.GetStringExtra("toDo");
                _toDo = JsonConvert.DeserializeObject<ToDo>(poiJson);
            }
            else
            {
                _toDo = new ToDo();
            }

            UpdateUI();
        }

        protected void UpdateUI()
        {
            _editHeading.Text = _toDo.heading;
            _editDescription.Text = _toDo.description;
            _editIsComplete.Checked = _toDo.is_complete;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.DetailMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.actionSave:
                    SaveToDo();
                    return true;
                case Resource.Id.actionDelete:
                    DeleteToDo();
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }


        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            base.OnPrepareOptionsMenu(menu);
            // Disable delete for a new TO Do
            if (_toDo.id <= 0)
            {
                IMenuItem item = menu.FindItem(Resource.Id.actionDelete);
                //disable menu item
                item.SetEnabled(false);

                //hide menu items
                item.SetVisible(false);
            }
            return true;
        }

        protected void SaveToDo()
        {
            bool errors = false;
            if (String.IsNullOrEmpty(_editHeading.Text))
            {
                _editHeading.Error = "Heading cannot be empty";
                errors = true;
            }
            else
            {
                _editHeading.Error = null;
            }
                                    

            if (errors)
            {
                return;
            }

            _toDo.heading = _editHeading.Text;
            _toDo.description = _editDescription.Text;
            _toDo.is_complete = _editIsComplete.Checked;

            CreateOrUpdateToDoAsync(_toDo);
        }

        private async void CreateOrUpdateToDoAsync(ToDo toDo)
        {
            ToDoServices service = new ToDoServices();
            if (!service.isConnected(this))
            {
                Toast toast = Toast.MakeText(this, "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
                return;
            }

            string response = await service.CreateOrUpdateToDoAsync(_toDo);
            if (!string.IsNullOrEmpty(response))
            {
                Toast toast = Toast.MakeText(this, String.Format("{0} saved.", _toDo.heading), ToastLength.Short);
                toast.Show();
                Finish();
            }
            else
            {
                Toast toast = Toast.MakeText(this, "Something went Wrong!", ToastLength.Short);
                toast.Show();
            }
        }

        protected void DeleteToDo()
        {
            AlertDialog.Builder alertConfirm = new AlertDialog.Builder(this);
            alertConfirm.SetTitle("Confirm delete");
            alertConfirm.SetCancelable(false);
            alertConfirm.SetPositiveButton("OK", ConfirmDelete);
            alertConfirm.SetNegativeButton("Cancel", delegate { });
            alertConfirm.SetMessage(String.Format("Are you sure you want to delete {0}?", _toDo.heading));
            alertConfirm.Show();

        }

        protected void ConfirmDelete(object sender, EventArgs e)
        {
            DeleteToDoAsync();
        }

        public async void DeleteToDoAsync()
        {
            ToDoServices service = new ToDoServices();
            if (!service.isConnected(this))
            {
                Toast toast = Toast.MakeText(this, "Not conntected to internet. Please check your device network settings.", ToastLength.Short);
                toast.Show();
                return;
            }

            string response = await service.DeleteToDoAsync(_toDo.id);
            if (!string.IsNullOrEmpty(response))
            {
                Toast toast = Toast.MakeText(this, String.Format("{0} deleted.", _toDo.heading), ToastLength.Short);
                toast.Show();

                Finish();
            }
            else
            {
                Toast toast = Toast.MakeText(this, "Something went Wrong!", ToastLength.Short);
                toast.Show();
            }
        }

    }
}