/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 * 
 * $Id: groupMembership.aspx.cs,v 1.6 2007/10/18 20:42:15 pbailey Exp $
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Mail;
using System.Text;
using System.Configuration;
//using Microsoft.Web.UI.WebControls;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;
using iLabs.ServiceBroker.Internal;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.admin
{
	/// <summary>
	/// Summary description for groupMembership.
	/// </summary>
	public partial class groupMembership : System.Web.UI.Page
	{
		AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
	
		string registrationMailAddress = ConfigurationSettings.AppSettings["registrationMailAddress"];

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Session["UserID"]==null)
				Response.Redirect("../login.aspx");

			if(!IsPostBack)
			{
				//Since both trees essentially have the same contents 
				//(except that the target groups tree doesn't have users, 
				//use 1 method to build both of them
				this.BuildTrees();
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
			this.ibtnCopyTo.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnCopyCB_Click);
			this.ibtnMoveTo.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveCB_Click);
			this.ibtnRemove.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnRemoveCB_Click);
			
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		/* This method builds both the trees. Since they basically have similar contents
		 * except for the fact that the group tree doesn't need to list the users.
		 * A node cannot be added to 2 trees at the same time. So a clone needs to be created.
		 * Hence we have 2 rootNodes (one for each tree).
		 */
		private void BuildTrees()
		{
			try
			{
                int rootID = wrapper.GetGroupIDWrapper(Group.ROOT);
                TreeNode rootNodeAgents = new TreeNode("ROOT", rootID.ToString(), "../img/GrantImages/root.gif");
                rootNodeAgents.Expanded = true;
                rootNodeAgents.SelectAction = TreeNodeSelectAction.None;

                TreeNode rootNodeGroups = new TreeNode("ROOT", rootID.ToString(), "../img/GrantImages/root.gif");
				rootNodeGroups.Expanded = true;
                rootNodeGroups.SelectAction = TreeNodeSelectAction.None;

			
				//nodes under root node (all these are groups)
				// since all are groups, the processing of these nodes is done separately here
				// as opposed to doing it in an AddAgentsRecursively(RootNode) call
				// This reduces the no. of database calls, made from isAgentUser
				int[] gIDs = AdministrativeAPI.ListSubgroupIDs (rootID);
				Group[] groups = wrapper.GetGroupsWrapper(gIDs);
                List<Group> gList = new List<Group>();
                gList.AddRange(groups);
                gList.Sort();
               

				// might want to do some sorting of the Groups List here -- works without sorting

				//foreach node under the root node:
				//1- Add that node to the tree
				//2- recursively add children nodes to the tree
				foreach (Group g in gList)
				{
					if (g.groupID!=0)
					{
						
                        TreeNode newNodeGroups = new TreeNode(g.groupName, g.groupID.ToString(),
                            "../img/GrantImages/Folder.gif");
                        newNodeGroups.ShowCheckBox = true;
                        newNodeGroups.SelectAction = TreeNodeSelectAction.None;
                        rootNodeGroups.ChildNodes.Add(newNodeGroups);
                        TreeNode newNodeAgents = new TreeNode(g.groupName, g.groupID.ToString(),
                            "../img/GrantImages/Folder.gif");
                        newNodeAgents.Expanded = true;
                        newNodeAgents.SelectAction = TreeNodeSelectAction.None;
						AddAgentsRecursively(newNodeAgents, newNodeGroups);
						rootNodeAgents.ChildNodes.Add(newNodeAgents);
						
					}
				}
				agentsTreeView.Nodes.Add(rootNodeAgents);
				groupsTreeView.Nodes.Add(rootNodeGroups);
			}
			catch (Exception ex)
			{
				string msg = "Exception: Cannot list groups. "+ex.Message+". "+ex.GetBaseException();
				lblResponse.Text = Utilities.FormatErrorMessage(msg);
				lblResponse.Visible = true;
			}
		}

		/* 
		 * We need to pass in 2 nodes here, since the Groups tree is different from the agents tree
		 * The nAgents will contain users in addition to group members.
		 * nGroups will only contain members that are groups.
		 */
		private void AddAgentsRecursively(TreeNode nAgents, TreeNode nGroups)
		{
			try
			{
				int[] childUserIDs = wrapper.ListUserIDsInGroupWrapper(Convert.ToInt32(nAgents.Value));
				int[] childGroupIDs = wrapper.ListSubgroupIDsWrapper(Convert.ToInt32(nAgents.Value));

				User[] usersList = wrapper.GetUsersWrapper(childUserIDs);
                List<User> childUsersList = new List<User>();
                childUsersList.AddRange(usersList);
                childUsersList.Sort();

				Group[] groupsList = wrapper.GetGroupsWrapper(childGroupIDs);
                List<Group> childGroupsList = new List<Group>();
                childGroupsList.AddRange(groupsList);
                childGroupsList.Sort();
				//might want to sort arraylist here -- works without sorting
				foreach (User u in childUsersList)
				{
					TreeNode childNode = new TreeNode(u.userName, u.userID.ToString(),
					    "../img/GrantImages/user.gif");
                    childNode.ShowCheckBox = true;
                    childNode.SelectAction = TreeNodeSelectAction.None;
                    childNode.Expanded = false;
					nAgents.ChildNodes.Add(childNode);
				}

				//might want to sort arraylist here -- works without sorting
                if (childGroupsList == null || childGroupsList.Count< 1)
                {
                    nAgents.Expanded = false;
                }
				foreach (Group g in childGroupsList)
				{
					if (g.groupID>0)
					{
						TreeNode childNode = new TreeNode(g.groupName,g.groupID.ToString(),
                            "../img/GrantImages/Folder.gif");
                        childNode.SelectAction = TreeNodeSelectAction.None;
                        childNode.Collapse();
                        TreeNode groupChildNode = new TreeNode(g.groupName, g.groupID.ToString(),
                            "../img/GrantImages/Folder.gif");
                        groupChildNode.ShowCheckBox = true;
                        groupChildNode.SelectAction = TreeNodeSelectAction.None;
						this.AddAgentsRecursively(childNode, groupChildNode);
						nAgents.ChildNodes.Add(childNode);
						nGroups.ChildNodes.Add(groupChildNode);
					}
				}
			}
			catch (Exception ex)
			{
				lblResponse.Text = Utilities.FormatErrorMessage("Cannot list users and groups. "+ex.GetBaseException());
                lblResponse.Visible = true;
			}
		}

		private void ExpandNode(TreeNodeCollection nodes, int nodeID)
		{
			foreach(TreeNode n in nodes)
			{
                
				if(Convert.ToInt32(n.Value) == nodeID)
				{
					n.Expanded=true;
					TreeNode parent = (TreeNode)n.Parent;
					//parent.Expanded = true;
					if (!parent.Text.Equals("ROOT"))
						ExpandParent(parent);
				}
				ExpandNode(n.ChildNodes, nodeID);
			}
		}
        private void ExpandNodes(TreeNodeCollection nodes, int[] nodeIDs)
        {
            foreach (TreeNode n in nodes)
            {
                int nID = Convert.ToInt32(n.Value);
                foreach (int nodeID in nodeIDs)
                {
                    if (nID == nodeID)
                    {
                        n.Expanded = true;
                        TreeNode parent = (TreeNode)n.Parent;
                        //parent.Expanded = true;
                        if (!parent.Text.Equals("ROOT"))
                            ExpandParent(parent);
                    }
                }
                ExpandNodes(n.ChildNodes, nodeIDs);
            }
        }

		private void ExpandParent(TreeNode n)
		{
			n.Expanded=true;
			TreeNode parent = (TreeNode)n.Parent;
			if (!parent.Text.Equals("ROOT"))
				ExpandParent(parent);
		}

		private void ibtnCopyTo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			lblResponse.Visible=false;
		    TreeNode agentNode = agentsTreeView.SelectedNode;
			TreeNode groupNode = groupsTreeView.SelectedNode;
            if(agentNode == null || groupNode == null){
                string msg = "Error: You must select an item from each list";
				lblResponse.Text = Utilities.FormatErrorMessage(msg);
				lblResponse.Visible = true;
                return;
            }
			int memberID = Convert.ToInt32(agentNode.Value);
			string memberName = agentNode.Text;
			int destinationID = Convert.ToInt32(groupNode.Value);
			string destinationName = groupNode.Text;
			
			// Note that because of the business logic, no agent can be copied to the ROOT node.
			// The business logic says that agents cannot simultaneuosly exist under the ROOT node
			// and under some other group node.
			if(!memberName.Equals(Group.ROOT) &&
				(memberID!= destinationID) &&
				!memberName.Equals(Group.NEWUSERGROUP) &&
				!memberName.Equals(Group.ORPHANEDGROUP) &&
				!memberName.Equals(Group.SUPERUSER))
			{
				try
				{
					//Does anyone want functionality to send email once a user has been moved to a new group.
					//This can get annoying, so maybe it should be configurable in web.config
					// If yes.. then put code in here  - CV, 2/15/05
					//Logic for this is as follows - if user get email address, otherwise get email addresses of all users in a group (GetUserIDsRecursively call?)
					
					if(wrapper.AddMemberToGroupWrapper(memberID, destinationID))
					{
						lblResponse.Visible=true;
						string msg = "'"+memberName + "' was successfully copied to '" + destinationName+"'.";
						//send email message to moved user/group if given access from a request group
						//Now get the ID of the group to remove the memberID from
						TreeNode parentNode = (TreeNode) agentNode.Parent;
						int parentGroupID = Convert.ToInt32(parentNode.Value);
						string parentGroupName = parentNode.Text;
						Group parentGroup = wrapper.GetGroupsWrapper(new int[] {parentGroupID})[0];

						if ((parentGroup.groupType.Equals(GroupType.REQUEST)) &&(wrapper.GetAssociatedGroupIDWrapper(parentGroupID)== destinationID))
						{
							MailMessage mail = new MailMessage();

							string email = "";
							if (InternalAuthorizationDB.IsAgentUser(memberID))
							{
								email = wrapper.GetUsersWrapper(new int[] {memberID})[0].email;
							}
							else
							{
								email = wrapper.GetGroupsWrapper(new int[] {memberID})[0].email;
							}
							mail.To = email;
							mail.From = registrationMailAddress;

							mail.Subject = "[iLabs] Request to join '" + destinationName+"' approved" ;
							mail.Body = "You have been given permission to access the '"+ destinationName +"' group.";
							mail.Body += "\n\r\n\r";
							mail.Body += "Login with the username and password that you registered with to use the lab.";
							mail.Body += "\n\r\n\r";	
							mail.Body += "-------------------------------------------------\n\r";
							mail.Body += "This is an automatically generated message. ";
							mail.Body += "DO NOT reply to the sender. \n\n";
							//mail.Body += "For questions regarding this service, email ilab-debug@mit.edu";
							SmtpMail.SmtpServer = "127.0.0.1";
							try
							{
								SmtpMail.Send(mail);
								msg+=" An email has been sent confirming the move." ;
								lblResponse.Text=Utilities.FormatConfirmationMessage(msg);
							}
							catch (Exception ex)
							{
								// Report detailed SMTP Errors
								StringBuilder smtpErrorMsg = new StringBuilder();
								smtpErrorMsg.Append("Exception: " + ex.Message);
								//check the InnerException
								if (ex.InnerException != null)
									smtpErrorMsg.Append("<br>Inner Exceptions:");
								while( ex.InnerException != null )
								{
									smtpErrorMsg.Append("<br>" +  ex.InnerException.Message);
									ex = ex.InnerException;
								}
								msg+=" However an error occurred while sending email to the member."+ ".<br>" + smtpErrorMsg.ToString();
								lblResponse.Text = Utilities.FormatErrorMessage(msg);
							}
						}
						else
						{
							lblResponse.Text=Utilities.FormatConfirmationMessage(msg);;
						}
						
						//Refresh tree views
						agentsTreeView.Nodes.Clear();
						groupsTreeView.Nodes.Clear();
						this.BuildTrees();

						//expand the user tree to show the state of the destination group
						//& select the node that was just moved
						ExpandNode(agentsTreeView.Nodes,destinationID);
					}
						
					else // addmembertogroup failed because the member was already a part of the group
					{
						lblResponse.Visible=true;
						string msg = "'"+memberName + "' could not be copied to '" + destinationName+"'";

						int[] parents = wrapper.ListGroupsForAgentWrapper(memberID);
						ArrayList parentList = new ArrayList(parents);
						if (parentList.Contains(destinationID))
							msg += ", since it already exists in '" + destinationName+"'";

						lblResponse.Text=Utilities.FormatErrorMessage(msg);
					}
				}
				catch(Exception ex)
				{
					lblResponse.Visible=true;
					string msg = "Exception thrown when trying to copy '" + memberName + "' to '" + destinationName+"'. "+ex.Message+". "+ex.GetBaseException();
					lblResponse.Text=Utilities.FormatErrorMessage(msg);

				}
			}
			else // if they're trying to transfer the ROOT, superUser etc.
			{
				lblResponse.Visible=true;
				string msg = "";
				if (memberID != destinationID)
				{
					msg = "The '"+memberName + "' group cannot be copied to another group.";
				}
				else
				{
					msg = "'"+memberName + "'  cannot be copied to itself.";
				}
				lblResponse.Text=Utilities.FormatErrorMessage(msg);
			}
		}

		private void ibtnMoveTo_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            bool status = false;
			lblResponse.Visible=false;
			TreeNode agentNode = agentsTreeView.SelectedNode;
            TreeNode groupNode = groupsTreeView.SelectedNode;
            if(agentNode == null || groupNode == null){
                string msg = "Error: You must select an item from each list";
				lblResponse.Text = Utilities.FormatErrorMessage(msg);
				lblResponse.Visible = true;
                return;
            }
			int memberID = Convert.ToInt32(agentNode.Value);
			string memberName = agentNode.Text;
			int destinationID = Convert.ToInt32(groupNode.Value);
			string destinationName = groupNode.Text;
			
			//Now get the ID of the group to remove the memberID from
			TreeNode parentNode = (TreeNode) agentNode.Parent;
			int parentGroupID = Convert.ToInt32(parentNode.Value);
			string parentGroupName = parentNode.Text;
			
			// Note that because of the business logic, no agent can be moved to the ROOT node.
			// The business logic says that agents cannot simultaneuosly exist under the ROOT node
			// and under some other group node.
			if(!memberName.Equals(Group.ROOT) &&
				(memberID!=destinationID) &&
				!memberName.Equals(Group.NEWUSERGROUP) &&
				!memberName.Equals(Group.ORPHANEDGROUP) &&
				!memberName.Equals(Group.SUPERUSER))
			{
                StringBuilder msg = new StringBuilder();
				try
				{
					if(wrapper.AddMemberToGroupWrapper(memberID, destinationID))
					{
                        status = true;
                        msg.Append("'" + memberName + "' was successfully added to '" + destinationName + "'");
						
						
						//send email message to moved user/group if given access from a request group
						Group parentGroup = wrapper.GetGroupsWrapper(new int[] {parentGroupID})[0];

                        if (parentGroupID == InternalAdminDB.SelectGroupID(Group.ORPHANEDGROUP))
                        {
                            status = true;
                        }
                        else
                        {
                            status = (wrapper.RemoveMembersFromGroupWrapper(new int[] { memberID }, parentGroupID).Length == 0);
                            if (status)
                            {
                                msg.Append(", and has been removed from '" + parentGroupName + "'.");
                            }
                            else
                            {
                                msg.Append(", but there was a problem removing the member from '" + parentGroupName + "'.");
                            }
                        }
                        //send email message to moved user/group if given access from a request group
                        if ((parentGroup.groupType.Equals(GroupType.REQUEST)) && (wrapper.GetAssociatedGroupIDWrapper(parentGroupID) == destinationID))
                        {
                            MailMessage mail = new MailMessage();

                            string email = "";
                            if (InternalAuthorizationDB.IsAgentUser(memberID))
                            {
                                email = wrapper.GetUsersWrapper(new int[] { memberID })[0].email;
                            }
                            else
                            {
                                email = wrapper.GetGroupsWrapper(new int[] { memberID })[0].email;
                            }
                            mail.To = email;
                            mail.From = registrationMailAddress;

                            mail.Subject = "[iLabs] Request to join '" + destinationName + "' approved";
                            mail.Body = "You have been given permission to access the '" + destinationName + "' group.";
                            mail.Body += "\n\r\n\r";
                            mail.Body += "Login with the username and password that you registered with to use the lab.";
                            mail.Body += "\n\r\n\r";
                            mail.Body += "-------------------------------------------------\n\r";
                            mail.Body += "This is an automatically generated message. ";
                            mail.Body += "DO NOT reply to the sender. \n\n";
                            //mail.Body += "For questions regarding this service, email ilab-debug@mit.edu";
                            SmtpMail.SmtpServer = "127.0.0.1";
                            try
                            {
                                SmtpMail.Send(mail);
                                msg.Append(" An email has been sent confirming the move.");

                            }
                            catch (Exception ex)
                            {
                                // Report detailed SMTP Errors
                                StringBuilder smtpErrorMsg = new StringBuilder();
                                smtpErrorMsg.Append("Exception: " + ex.Message);
                                //check the InnerException
                                if (ex.InnerException != null)
                                    smtpErrorMsg.Append("<br>Inner Exceptions:");
                                while (ex.InnerException != null)
                                {
                                    smtpErrorMsg.Append("<br>" + ex.InnerException.Message);
                                    ex = ex.InnerException;
                                }
                                msg.Append(" However an error occurred while sending email to the member." + ".<br>" + smtpErrorMsg.ToString());

                            }
                        }

                        if (status)
                        {
                            lblResponse.Text = Utilities.FormatConfirmationMessage(msg.ToString());
                        }
                        else
                        {
                            lblResponse.Text = Utilities.FormatErrorMessage(msg.ToString());
                        }
				
						//Refresh tree views
						agentsTreeView.Nodes.Clear();
						groupsTreeView.Nodes.Clear();
						this.BuildTrees();

						//expand the user tree to show the state of the destination group
						//& select the node that was just moved
						ExpandNode(agentsTreeView.Nodes,destinationID);
					}
						
					else // addmembertogroup failed because the member was already a part of the group
					{
						lblResponse.Visible=true;
						string errmsg = "'"+memberName + "' could not be moved to '" + destinationName+"' from '"+parentGroupName+"'";

						int[] parents = wrapper.ListGroupsForAgentWrapper(memberID);
						ArrayList parentList = new ArrayList(parents);
						if (parentList.Contains(destinationID))
							errmsg += ", since it already exists in '" + destinationName+"'";

						lblResponse.Text=Utilities.FormatErrorMessage(errmsg);
					}
				}
				catch(Exception ex)
				{
					//Should ideally roll back, but if there's a problem in removemembers then rollback isn't possible in any case!
					//wrapper.RemoveMembersFromGroupWrapper(new int[] {memberID},destinationID);
					lblResponse.Visible=true;
					string errmsg = "Exception thrown when trying to move " + memberName + " to '" + destinationName+"' from '"+parentGroupName+"'. "+ex.Message+". "+ex.GetBaseException();
					lblResponse.Text=Utilities.FormatErrorMessage(errmsg);

				}
			}
			else // if they're trying to transfer the ROOT, superUser etc.
			{
				lblResponse.Visible=true;
				string errmsg = "";
				if (memberID != destinationID)
				{
					errmsg = "The '"+memberName + "' group cannot be moved to another group.";
				}
				else
				{
					errmsg = "'"+memberName + "'  cannot be moved to itself.";
				}
				lblResponse.Text=Utilities.FormatErrorMessage(errmsg);
			}
		}

		private void ibtnRemove_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
            StringBuilder msg = new StringBuilder();
			lblResponse.Visible=false;
			TreeNode agentNode = agentsTreeView.SelectedNode;
            TreeNode groupNode = groupsTreeView.SelectedNode;
            if(agentNode == null || groupNode == null){
                msg.Append("Error: You must select an item from each list");
				lblResponse.Text = Utilities.FormatErrorMessage(msg.ToString());
				lblResponse.Visible = true;
                return;
            }
			int memberID = Convert.ToInt32(agentNode.Value);
			string memberName = agentNode.Text;
			int parentID = Convert.ToInt32(groupNode.Value);
			string parentName = groupNode.Text;
			
			// Note that because of the business logic, no agent can be copied to the ROOT node.
			// The business logic says that agents cannot simultaneuosly exist under the ROOT node
			// and under some other group node.
			if(memberName.Equals(Group.ROOT) ||
				(memberID== parentID)||
				(memberName.Equals(Group.NEWUSERGROUP)) ||
				(memberName.Equals(Group.ORPHANEDGROUP)) ||
				(memberName.Equals(Group.SUPERUSER)))
			{
				// if they're trying to transfer the ROOT, superUser etc.
				if (memberID != parentID)
				{
					msg.Append( "The '"+memberName + "' group cannot be removed from the system.");
				}
				else //if parent=groupname
				{
					msg.Append("'"+memberName + "'  cannot be removed from itself.");
				}
				lblResponse.Text=Utilities.FormatErrorMessage(msg.ToString());
                lblResponse.Visible = true;
			}
			else
			{
				try
				{
					//Does anyone want functionality to send email once a user has been moved to a new group.
					//This can get annoying, so maybe it should be configurable in web.config
					// If yes.. then put code in here  - CV, 2/15/05
                    //Logic for this is as follows - if user get email address, otherwise get email addresses of all users in a group (GetUserIDsRecursively call?)
					
					//if it has removed all the members
					if(wrapper.RemoveMembersFromGroupWrapper(new int[] {memberID}, parentID).Length==0)
					{
						lblResponse.Visible=true;
						msg.Append("'"+memberName + "' was successfully removed from '" + parentName+"'.");
						lblResponse.Text=Utilities.FormatConfirmationMessage(msg.ToString());
					
						//Refresh tree views
						agentsTreeView.Nodes.Clear();
						groupsTreeView.Nodes.Clear();
						this.BuildTrees();

						//expand the user tree to show the state of the parent group
						ExpandNode (agentsTreeView.Nodes, parentID);
					}
						
					else 
					{
						lblResponse.Visible=true;
						

						int[] parents = wrapper.ListGroupsForAgentWrapper(memberID);
						ArrayList parentList = new ArrayList(parents);
						if (parentList.Contains(parentID))
						{
							if (parentName.Equals(Group.ROOT))
								// then it's only parent was ROOT & it cannot be removed here
								msg.Append("The only group '"+memberName+"' belongs to is ROOT. It cannot be removed from ROOT using the 'Group Membership' functionality. Use the 'Delete' functionality in the User/Groups pages instead.");
							else
								msg.Append("'"+memberName+"' could not be removed from '"+parentName+"'");
						}
						else
							// remove member failed because the member was not part of the group
							msg.Append("'"+memberName + "' does not belong to '"+ parentName+"' and cannot be removed from it.");

						
						lblResponse.Text=Utilities.FormatErrorMessage(msg.ToString());
					}
				}
				catch(Exception ex)
				{
					lblResponse.Visible=true;
					msg.Append("Exception thrown when trying to remove '" + memberName + "' from '" + parentName+"'. "+ex.Message+". " +ex.GetBaseException());
					lblResponse.Text=Utilities.FormatErrorMessage(msg.ToString());
				}
			}
		}

        private void ibtnRemoveCB_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            lblResponse.Visible = false;
            lblResponse.Text = "";
            StringBuilder message = new StringBuilder();
            List<int> parentIDs = new List<int>();
            
            TreeNodeCollection agentNodes = agentsTreeView.CheckedNodes;
            if (agentNodes == null || agentNodes.Count < 1)
            {
                string msg = "Error: You must select agents to remove.";
                lblResponse.Text = Utilities.FormatErrorMessage(msg);
                lblResponse.Visible = true;
                return;
            }
            message.Append(removeAgentNodes(agentNodes, ref parentIDs));
            if(message.Length > 0){
                lblResponse.Text = Utilities.FormatConfirmationMessage(message.ToString());
                lblResponse.Visible = true;
            }
            //Refresh tree views
            agentsTreeView.Nodes.Clear();
            groupsTreeView.Nodes.Clear();
            this.BuildTrees();

            //expand the user tree to show the state of the parent group
            foreach (int parentID in parentIDs)
            {
                ExpandNode(agentsTreeView.Nodes, parentID);
            }

        }
       

         private void ibtnCopyCB_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            lblResponse.Visible = false;
            lblResponse.Text = "";
            copyAgents(false);
        }

        private void ibtnMoveCB_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            lblResponse.Visible = false;
            lblResponse.Text = "";
            copyAgents(true);
        }

        protected void btnSearch_Click(object sender, System.EventArgs e)
        {
        }


        private string removeAgentNodes(TreeNodeCollection agentNodes, ref List<int> parentIDs)
        {
            StringBuilder msg = new StringBuilder();
            List<int> agents = new List<int>();
            foreach (TreeNode agentNode in agentNodes)
            {
                int memberID = Convert.ToInt32(agentNode.Value);
                string memberName = agentNode.Text;
                int parentID = Convert.ToInt32(agentNode.Parent.Value);
                string parentName = agentNode.Parent.Text;

                // Note that because of the business logic you may not remove built-in groups
                if (memberName.Equals(Group.ROOT)
                    || (memberID == parentID)
                    || (memberName.Equals(Group.NEWUSERGROUP))
                    || (memberName.Equals(Group.ORPHANEDGROUP))
                    || (memberName.Equals(Group.SUPERUSER)))
                {
                    // if they're trying to remove the ROOT, superUser etc.
                    if (memberID != parentID)
                    {
                        msg.Append("The '" + memberName + "' group cannot be removed from the system.<br />");
                    }
                    else //if parent=groupname
                    {
                        msg.Append("'" + memberName + "'  cannot be removed from itself.<br />");
                    }
                }
                else
                {
                    try
                    {
                        //Does anyone want functionality to send email once a user has been moved to a new group.
                        //This can get annoying, so maybe it should be configurable in web.config
                        // If yes.. then put code in here  - CV, 2/15/05
                        //Logic for this is as follows - if user get email address, otherwise get email addresses of all users in a group (GetUserIDsRecursively call?)

                        //if it has removed all the members
                        if (wrapper.RemoveMembersFromGroupWrapper(new int[] { memberID }, parentID).Length == 0)
                        {
                            if (!agents.Contains(memberID))
                                agents.Add(memberID);
                            if (!parentIDs.Contains(parentID))
                                parentIDs.Add(memberID);
                            lblResponse.Visible = true;
                            msg.Append("'" + memberName + "' was successfully removed from '" + parentName + "'.<br />");
                        }
                    }
                    catch (Exception removeEx)
                    {
                        msg.Append("ERROR: removing '" + memberName + "' from '" + parentName + "'. Exception: " + removeEx.Message + "<br />");
                    }
                }
            } // END foreach agent

            foreach (int id in agents)
            {
                int[] parents = wrapper.ListGroupsForAgentWrapper(id);
                if ((parents == null) || (parents.Length == 0))
                {
                    wrapper.AddMemberToGroupWrapper(id, InternalAdminDB.SelectGroupID(Group.ORPHANEDGROUP));
                }
            }
            return msg.ToString();
        }

        private void copyAgents(bool move)
        {
            StringBuilder msg = new StringBuilder();
            List<int> expandIDs = new List<int>();
            lblResponse.Visible = false;
            TreeNodeCollection agentNodes = agentsTreeView.CheckedNodes;
            TreeNodeCollection groupNodes = groupsTreeView.CheckedNodes;
            if (agentNodes == null || agentNodes.Count < 1 || groupNodes == null || groupNodes.Count < 1)
            {
                msg.Append("Error: You must check at least one item from each list");
                lblResponse.Text = Utilities.FormatErrorMessage(msg.ToString());
                lblResponse.Visible = true;
                return;
            }
            foreach (TreeNode agentNode in agentNodes)
            {
                int count = 0;
                bool status = false;
                int agentID = Convert.ToInt32(agentNode.Value);
                string agentName = agentNode.Text;

                //Now get the ID of the Parent group
                TreeNode parentNode = (TreeNode)agentNode.Parent;
                Group parentGroup = wrapper.GetGroupsWrapper(new int[] { Convert.ToInt32(parentNode.Value) })[0];

                foreach (TreeNode groupNode in groupNodes)
                {
                    int destinationID = Convert.ToInt32(groupNode.Value);
                    string destinationName = groupNode.Text;


                    // Note that because of the business logic, no agent can be moved to the ROOT node.
                    // The business logic says that agents cannot simultaneuosly exist under the ROOT node
                    // and under some other group node.
                    if (agentName.Equals(Group.ROOT)
                       || (agentID == destinationID)
                       || agentName.Equals(Group.NEWUSERGROUP)
                       || agentName.Equals(Group.ORPHANEDGROUP)
                       || agentName.Equals(Group.SUPERUSER))
                    {
                        msg.Append("'ERROR: You may not copy/move " + agentName + "' to '" + destinationName + "'<br />");
                    }
                    else
                    {

                        try
                        {
                            if (AdministrativeAPI.IsAgentMember(agentID, destinationID))
                            {
                                msg.Append("Warning: '" + agentName + "' is already a member of '" + destinationName + "'<br />");
                            }
                            else
                            {
                                if (wrapper.AddMemberToGroupWrapper(agentID, destinationID))
                                {
                                    count++;
                                    if (!expandIDs.Contains(destinationID))
                                        expandIDs.Add(destinationID);
                                    msg.Append("'" + agentName + "' was successfully added to '" + destinationName + "'<br />");

                                    //send email message to moved user/group if given access from a request group
                                    if ((parentGroup.groupType.Equals(GroupType.REQUEST)) && (wrapper.GetAssociatedGroupIDWrapper(parentGroup.GroupID) == destinationID))
                                    {
                                        MailMessage mail = new MailMessage();

                                        string email = "";
                                        if (InternalAuthorizationDB.IsAgentUser(agentID))
                                        {
                                            email = wrapper.GetUsersWrapper(new int[] { agentID })[0].email;
                                        }
                                        else
                                        {
                                            email = wrapper.GetGroupsWrapper(new int[] { agentID })[0].email;
                                        }
                                        mail.To = email;
                                        mail.From = registrationMailAddress;

                                        mail.Subject = "[iLabs] Request to join '" + destinationName + "' approved";
                                        mail.Body = "You have been given permission to access the '" + destinationName + "' group.";
                                        mail.Body += "\n\r\n\r";
                                        mail.Body += "Login with the username and password that you registered with to use the lab.";
                                        mail.Body += "\n\r\n\r";
                                        mail.Body += "-------------------------------------------------\n\r";
                                        mail.Body += "This is an automatically generated message. ";
                                        mail.Body += "DO NOT reply to the sender. \n\n";
                                        //mail.Body += "For questions regarding this service, email ilab-debug@mit.edu";
                                        SmtpMail.SmtpServer = "127.0.0.1";
                                        try
                                        {
                                            SmtpMail.Send(mail);
                                            msg.Append(" An email has been sent confirming the move.<br />");

                                        }
                                        catch (Exception ex)
                                        {
                                            // Report detailed SMTP Errors
                                            StringBuilder smtpErrorMsg = new StringBuilder();
                                            smtpErrorMsg.Append("Exception: " + ex.Message);
                                            //check the InnerException
                                            if (ex.InnerException != null)
                                                smtpErrorMsg.Append("<br>Inner Exceptions:");
                                            while (ex.InnerException != null)
                                            {
                                                smtpErrorMsg.Append("<br>" + ex.InnerException.Message);
                                                ex = ex.InnerException;
                                            }
                                            msg.Append(" However an error occurred while sending email to the member." + ". Exception: " + smtpErrorMsg.ToString() + "<br />");

                                        }
                                    } //End Send Mail
                                }
                                else
                                {
                                    msg.Append("'ERROR: adding " + agentName + "' to '" + destinationName + "'<br />");
                                }
                            }
                        }// END try Add member
                        catch (Exception addEx)
                        {
                            msg.Append("Error: Adding " + agentName + "' to '" + destinationName + "' Exception: " + addEx.Message + "<br />");
                        }
                    }
                } 
            } // END Agents
            if (move) // Made at least one copy
            {
                msg.Append(removeAgentNodes(agentNodes, ref expandIDs));
            }
            //Refresh tree views
            agentsTreeView.Nodes.Clear();
            groupsTreeView.Nodes.Clear();
            this.BuildTrees();

            //expand the user tree to show the state of the destination group
            //& select the node that was just moved
                foreach(int id in expandIDs){
            ExpandNode(agentsTreeView.Nodes, id);
                }
            lblResponse.Text = Utilities.FormatConfirmationMessage(msg.ToString());
            lblResponse.Visible = true;
        }

       

	}
}
