/*
 * Copyright (c) 2004 The Massachusetts Institute of Technology. All rights reserved.
 * Please see license.txt in top level directory for full license.
 */

/* $Id: assocLabServersPopup.aspx.cs,v 1.8 2007/05/16 19:07:16 pbailey Exp $ */

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using iLabs.Core;
using iLabs.DataTypes.ProcessAgentTypes;
using iLabs.DataTypes.TicketingTypes;
using iLabs.ServiceBroker;
using iLabs.ServiceBroker.Internal;
using iLabs.ServiceBroker.Administration;
using iLabs.ServiceBroker.Authorization;

using iLabs.Ticketing;
using iLabs.UtilLib;

namespace iLabs.ServiceBroker.admin
{
	/// <summary>
	/// Summary description for assocLabServersPopup.
	/// </summary>
	public partial class assocLabServersPopup : System.Web.UI.Page
	{
		//The error message div tab

        AuthorizationWrapperClass wrapper = new AuthorizationWrapperClass();
		int labClientID;
		int[] labClientIDs;
		LabClient[] labClients;

		int[] labServerIDs;
		ProcessAgentInfo[] AssocLabServers;

		ProcessAgentInfo[] AvailLabServers;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Session["UserID"]==null)
				Response.Redirect("../login.aspx");

			labClientID = int.Parse(Request.Params["lc"]);
			labClientIDs = new int[1];
			labClientIDs[0] = labClientID;
			labClients = wrapper.GetLabClientsWrapper(labClientIDs);

			// Get Available Lab Servers
            BrokerDB brokerDB = new BrokerDB();
            int[] serverIDs = brokerDB.GetProcessAgentIDsByType(new int[]{
                (int)ProcessAgentType.AgentType.LAB_SERVER,
                (int)ProcessAgentType.AgentType.BATCH_LAB_SERVER});
			AvailLabServers = wrapper.GetProcessAgentInfosWrapper(serverIDs);

			/// Get Associated Lab Servers (i.e. associated to a specified Lab Client)			
			AssocLabServers = wrapper.GetProcessAgentInfosWrapper(labClients[0].labServerIDs);

			// Current Lab Client name
			lblLabClient.Text = labClients[0].clientName;

			// Error Message
			divErrorMessage.Visible = false;
			lblResponse.Visible = false;

			if(!IsPostBack)
			{
				LoadListBoxes();
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
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.ibtnAdd.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnAdd_Click);
			this.ibtnRemove.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnRemove_Click);
			this.ibtnMoveUp.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveUp_Click);
			this.ibtnMoveDown.Click += new System.Web.UI.ImageClickEventHandler(this.ibtnMoveDown_Click);

		}
		#endregion

		/// <summary>
		/// Write Available and Associated Lab Servers List Boxes
		/// </summary>
		private void LoadListBoxes()
		{
			int i;

			// Clear listboxes
			lbxAvailable.Items.Clear();
			lbxAssociated.Items.Clear();

			// Create empty List Items in the Associated Lab Server list box.
			// These have to be pre-created (can't just use the Add method) in order
			// to preserve the display order in the list box.
			for (i=0; i<AssocLabServers.Length; i++)
			{
				lbxAssociated.Items.Add(new ListItem());
			}

			bool associated;

			foreach(ProcessAgentInfo si in AvailLabServers)
			{
				if ((si.agentId == 0) || si.retired)
					continue;
			
				associated = false;
				
				// Don't write Lab Servers that are already associated with the client
				// to the Available List box

				i = 0; // index to preserve display order (see below)
				foreach(ProcessAgentInfo si2 in AssocLabServers)
				{
					if(si.agentId == si2.agentId)
					{
						associated = true;
						break;
					}
					i ++;
				}

				if(associated)
				{
					//Write to Associated Listbox if already associated.

					// An indexing scheme is used here in order to preserve the display order
					// of the associated items. Without this, they would appear in the list box
					// in the order they arrived during the iteration through the available lab servers.
					lbxAssociated.Items[i].Text = si.webServiceUrl;
					lbxAssociated.Items[i].Value = si.agentId.ToString();

				}
				else
				{
					//Write to Available Listbox if not associated
                    if(!si.retired)
					    lbxAvailable.Items.Add(new ListItem(si.webServiceUrl, si.agentId.ToString()));
				}

			}

		}

		/// <summary>
		/// Add Button.
		/// Adds items from the Available Listbox to the Associated Listbox.
		/// Removes added items from the Available Listbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ibtnAdd_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (lbxAvailable.SelectedIndex >= 0)
			{
				ListItem li = lbxAvailable.SelectedItem;

				lbxAvailable.Items.Remove(li);
				//lbxAvailable.SelectedIndex = 0;

				lbxAssociated.SelectedIndex = -1;
				lbxAssociated.Items.Add(li);
			}
		}

		/// <summary>
		/// Remove Button.
		/// Removes items from the Associated Listbox.
		/// Adds removed items back to the Available Listbox.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ibtnRemove_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			if (lbxAssociated.SelectedIndex >= 0)
			{
				ListItem li = lbxAssociated.SelectedItem;

                //lbxAssociated.SelectedIndex = 0;
				lbxAssociated.Items.Remove(li);
				
				lbxAvailable.SelectedIndex = -1;
				lbxAvailable.Items.Add(li);
			}
		}

		/// <summary>
		/// Move Up Button.
		/// Moves an item in the Associated Listbox up one position.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ibtnMoveUp_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			int i;
			if ( (i = lbxAssociated.SelectedIndex) >0 ) 
			{
				ListItem li1 = lbxAssociated.Items[i];
				ListItem li2 = lbxAssociated.Items[i-1];
				lbxAssociated.Items.Remove(li1);
				lbxAssociated.Items.Remove(li2);
				lbxAssociated.Items.Insert(i-1,li1);
				lbxAssociated.Items.Insert(i,li2);
			}
		
		}

		/// <summary>
		/// Move Down button.
		/// Moves an item in the Associated List Box down one position.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ibtnMoveDown_Click(object sender, System.Web.UI.ImageClickEventArgs e)
		{
			int i=lbxAssociated.SelectedIndex;
			if (i >= 0 && i < lbxAssociated.Items.Count-1) 
			{
				ListItem li1=lbxAssociated.Items[i];
				ListItem li2=lbxAssociated.Items[i+1];
				lbxAssociated.Items.Remove(li1);
				lbxAssociated.Items.Remove(li2);
				lbxAssociated.Items.Insert(i,li1);
				lbxAssociated.Items.Insert(i,li2);
			}
		}

		/// <summary>
		/// Save Button.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnSaveChanges_Click(object sender, System.EventArgs e)
		{
			//load the labClientIDs integer array from the Associated Lab Servers listbox
			labServerIDs = new int[lbxAssociated.Items.Count];
			for(int i = 0; i < lbxAssociated.Items.Count; i++)
			{
				labServerIDs[i] = int.Parse(lbxAssociated.Items[i].Value);
			}

			// Update the Lab Client Record with the new Lab Server list (labServerIDs)
			try
			{
				//1. find groups that can access this lab server
				int[] groupIDs = AdministrativeUtilities.GetLabClientGroups(labClientID);

				ArrayList oldUseLSGrants = new ArrayList();
				// First delete all "uselabserver" grants for the groups that can access the old set of lab servers
				foreach (int groupID in groupIDs)
				{
					foreach (int labServerID in labClients[0].labServerIDs)
					{
						int qID = AuthorizationAPI.GetQualifierID(labServerID, Qualifier.labServerQualifierTypeID);
						int[] oldLSGrants = AuthorizationAPI.FindGrants(groupID, Function.useLabServerFunctionType,qID);
						foreach (int oldGrant in oldLSGrants)
							oldUseLSGrants.Add(oldGrant);
					}
				}

				// remove all the grants
				int[] unremovedGrants = AuthorizationAPI.RemoveGrants(Utilities.ArrayListToIntArray(oldUseLSGrants));

				//Change the labclient's list of lab servers
                wrapper.ModifyLabClientWrapper(labClientID, labClients[0].clientName, labClients[0].version, 
                    labClients[0].clientShortDescription, labClients[0].clientLongDescription, 
                    labClients[0].notes, labClients[0].loaderScript, labClients[0].clientType, 
                    labServerIDs, labClients[0].contactEmail, labClients[0].contactFirstName, 
                    labClients[0].contactLastName, labClients[0].needsScheduling, labClients[0].needsESS,
                    labClients[0].IsReentrant, labClients[0].clientInfos);
				
				// Create the javascript which will cause a page refresh event to fire on the popup's parent page
				string jScript;
				jScript = "<script language=javascript> window.opener.Form1.hiddenPopupOnSave.value='1';";
				jScript += "window.close();</script>";
				Page.RegisterClientScriptBlock("postbackScript", jScript);
			
				// add uselabserver grants for agents that have uselabclientgrants
		

				//1. assign uselabserver grants for each group
				foreach (int labServerID in labServerIDs)
				{
					int qID = AuthorizationAPI.GetQualifierID(labServerID, Qualifier.labServerQualifierTypeID);
					foreach (int groupID in groupIDs)
						AuthorizationAPI.AddGrant(groupID, Function.useLabServerFunctionType,qID);
				}

				lblResponse.Visible = true;
				lblResponse.Text = Utilities.FormatConfirmationMessage("The labclient '"+labClients[0]+"' has successfully been modified.");
			}
			catch (Exception ex)
			{
				divErrorMessage.Visible = true;
				lblResponse.Visible = true;
				lblResponse.Text = "Cannot update Lab Client. " + ex.Message;

			}

		
		}
	}
}
