using System;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SecureDataInterchange.Admin.Common.UserControls
{
    public partial class UserList : UserControl
    {
        private const string NAME_GRIDMODE = "GridMode";
        private const string NAME_SELECTEDUSERS = "SelectedUsers";
        private int rowCount = -1;

        public enum GridMode
        {
            None,
            CheckUserAllowed,
            LinkToEditUserAllowed
        }

        public UserList.GridMode Mode
        {
            get
            {
                bool flag = this.ViewState["GridMode"] != null;
                UserList.GridMode gridMode;
                if (flag)
                {
                    gridMode = (UserList.GridMode)this.ViewState["GridMode"];
                }
                else
                {
                    gridMode = UserList.GridMode.None;
                }
                return gridMode;
            }
            set
            {
                bool flag = this.Mode == UserList.GridMode.None;
                if (flag)
                {
                    this.ViewState.Add("GridMode", value);
                }
                else
                {
                    this.ViewState["GridMode"] = value;
                }
            }
        }

        public MembershipUserCollection SelectedUsers
        {
            get
            {
                return this._selectedUsers;
            }
        }

        private MembershipUserCollection _selectedUsers
        {
            get
            {
                bool flag = this.ViewState["SelectedUsers"] == null;
                MembershipUserCollection muc;
                if (flag)
                {
                    muc = new MembershipUserCollection();
                    this.ViewState.Add("SelectedUsers", muc);
                }
                else
                {
                    muc = (MembershipUserCollection)this.ViewState["SelectedUsers"];
                }
                return muc;
            }
            set
            {
                bool flag = this.ViewState["SelectedUsers"] == null;
                if (flag)
                {
                    this.ViewState.Add("SelectedUsers", value);
                }
                else
                {
                    this.ViewState["SelectedUsers"] = value;
                }
            }
        }

        public void SetMode(UserList.GridMode mode)
        {
            this.Mode = mode;
        }

        public void ClearSelectedUsers()
        {
            this._selectedUsers = new MembershipUserCollection();
            foreach (GridViewRow gridViewRow in this.gvUsers.Rows)
            {
                CheckBox includeCheckBox = gridViewRow.FindControl("chkInclude") as CheckBox;
                if (includeCheckBox != null)
                {
                    includeCheckBox.Checked = false;
                }
            }
        }

        protected void gvUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.SearchByEmailAddress(this.userNameSearchTextBox.Text);
            this.gvUsers.PageIndex = e.NewPageIndex;
            this.gvUsers.DataBind();
        }

        protected void InitiateSearchButton_Click(object sender, EventArgs e)
        {
            this.SearchByEmailAddress(this.userNameSearchTextBox.Text);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.SaveSelectedUsers();
        }

        protected void Users_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            bool flag = e.Row.RowType != DataControlRowType.Pager && e.Row.RowType != DataControlRowType.EmptyDataRow;
            if (flag)
            {
                HyperLink hlUser = (HyperLink)e.Row.FindControl("hlUser");
                CheckBox chkInclude = (CheckBox)e.Row.FindControl("chkInclude");
                bool flag2 = this.Mode != UserList.GridMode.CheckUserAllowed && this.rowCount > 0;
                if (flag2)
                {
                    e.Row.Cells[0].Visible = false;
                }
                bool flag3 = hlUser != null && (this.Mode == UserList.GridMode.None || this.Mode == UserList.GridMode.CheckUserAllowed);
                if (flag3)
                {
                    hlUser.NavigateUrl = null;
                }
                bool flag4 = chkInclude != null && hlUser != null;
                if (flag4)
                {
                    bool flag5 = this._selectedUsers[hlUser.Text] != null;
                    if (flag5)
                    {
                        chkInclude.Checked = true;
                    }
                }
            }
        }

        private void SaveSelectedUsers()
        {
            bool flag = this.Mode == UserList.GridMode.CheckUserAllowed;
            if (flag)
            {
                foreach (object obj in this.gvUsers.Rows)
                {
                    GridViewRow gvr = (GridViewRow)obj;
                    CheckBox chkInclude = (CheckBox)gvr.FindControl("chkInclude");
                    HyperLink hlUser = (HyperLink)gvr.FindControl("hlUser");
                    bool @checked = chkInclude.Checked;
                    if (@checked)
                    {
                        bool flag2 = this._selectedUsers[hlUser.Text] == null;
                        if (flag2)
                        {
                            this._selectedUsers.Add(Membership.GetUser(hlUser.Text));
                        }
                    }
                    else
                    {
                        bool flag3 = this._selectedUsers[hlUser.Text] != null;
                        if (flag3)
                        {
                            this._selectedUsers.Remove(hlUser.Text);
                        }
                    }
                }
            }
        }

        private void SearchByEmailAddress(string emailAddressSearchString)
        {
            MembershipUserCollection muc = Membership.FindUsersByName("%" + emailAddressSearchString + "%");
            this.rowCount = muc.Count;
            bool flag = this.rowCount > 0;
            if (flag)
            {
                this.gvUsers.PageIndex = 0;
                this.gvUsers.DataSource = muc;
                this.gvUsers.DataBind();
                this.gvUsers.Visible = true;
                this.lblNoUsers.Visible = false;
            }
            else
            {
                this.gvUsers.Visible = false;
                this.lblNoUsers.Visible = true;
            }
        }
    }
}
