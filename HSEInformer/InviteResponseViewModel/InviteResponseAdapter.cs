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
using HSEInformer.GroupMemberViewModel;
using HSEInformer.GroupViewModel;

namespace HSEInformer.InviteResponseViewModel
{
    class InviteResponseAdapter : RecyclerView.Adapter
    {
        public GroupList _groupList;

        public event Action<Model.Group, bool> ItemClick;

        public InviteResponseAdapter(GroupList groupList)
        {
            _groupList = groupList;

        }

        public override int ItemCount
        {
            get { return _groupList.NumGroups; }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            RecyclerView.ViewHolder rh;
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.InviteResponseView, parent, false);
            rh = new InviteResponseViewHolder(itemView, OnClick);
            return rh;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            //Modifying base view with exact data
            if (holder is InviteResponseViewHolder)
            {
                InviteResponseViewHolder rh = holder as InviteResponseViewHolder;
                rh.GroupTextView.Text = _groupList[position].Name;
            }
        }

        void OnClick(int postion, bool accepted)
        {
            ItemClick?.Invoke(_groupList[postion], accepted);
        }
    }
}