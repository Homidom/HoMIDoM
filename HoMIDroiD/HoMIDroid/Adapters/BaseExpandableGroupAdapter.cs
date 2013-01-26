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
using HoMIDroid.BO;

namespace HoMIDroid.Adapters
{
    public abstract class BaseExpandableGroupAdapter<T> : BaseExpandableListAdapter where T: BaseObject
    {
        public Context Context { get; private set; }
        public List<Group<T>> Groups { get; private set; }

        public BaseExpandableGroupAdapter(Context context, List<Group<T>> groups)
        {
            this.Groups = groups;
            this.Context = context;
        }


        public override int GroupCount
        {
            get { return this.Groups.Count; }
        }


        public override bool HasStableIds
        {
            get { return true; }
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return true;
        }
        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return this.Groups[groupPosition].Elements[childPosition];
        }
        public override long GetChildId(int groupPosition, int childPosition)
        {
            return childPosition;
        }
        public override int GetChildrenCount(int groupPosition)
        {
            return this.Groups[groupPosition].Elements.Count();
        }
        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return this.Groups[groupPosition];
        }
        public override long GetGroupId(int groupPosition)
        {
            return groupPosition;
        }

    }
}