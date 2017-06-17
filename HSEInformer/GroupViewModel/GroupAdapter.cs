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
using Android.Support.V7.Widget;

namespace HSEInformer.GroupViewModel
{
    class GroupAdapter : RecyclerView.Adapter
    {
        public GroupList _groupList;
        Context context;

        public event Action<Model.Group> ItemClick;

        public GroupAdapter(Context context, GroupList groupList)
        {
            _groupList = groupList;
            this.context = context;

        }

        public override int ItemCount
        {
            get { return _groupList.NumGroups; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.GroupView, parent, false);
            rh = new GroupViewHolder(itemView, OnClick);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is GroupViewHolder)
            {
                GroupViewHolder rh = holder as GroupViewHolder;
                rh.ContentButton.Text = _groupList[position].Name;
            }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(_groupList[position ]);
        }

    }
}