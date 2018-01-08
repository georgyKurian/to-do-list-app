using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;

namespace To_Do_List
{
    class ToDoListViewAdapter: BaseAdapter<ToDo>
    {
        private readonly Activity context;
        private List<ToDo> toDoListData;
        private CheckBox _checkbox;

        public ToDoListViewAdapter(Activity _context, List<ToDo> _list)
            : base()
        {
            this.context = _context;
            this.toDoListData = _list;
        }

        public override int Count
        {
            get { return toDoListData.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override ToDo this[int index]
        {
            get { return toDoListData[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            // re-use an existing view, if one is available otherwise create a new one
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.ToDoItem, null, false);
            }

            ToDo toDo = this[position];
            view.FindViewById<TextView>(Resource.Id.itemHeading).Text = toDo.heading;
            _checkbox = view.FindViewById<CheckBox>(Resource.Id.itemCheckbox);

            _checkbox.Checked = toDo.is_complete;
            _checkbox.Focusable = false;
            _checkbox.FocusableInTouchMode = false;
            _checkbox.Clickable = true;

            return view;
        }
    }
}