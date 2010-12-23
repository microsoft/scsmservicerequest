//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.EnterpriseManagement;
using Microsoft.EnterpriseManagement.Common;
using Microsoft.EnterpriseManagement.ConnectorFramework;
using Microsoft.EnterpriseManagement.ConsoleFramework;
using Microsoft.EnterpriseManagement.UI.SdkDataAccess;
// Requires Microsoft.EnterpriseManagement.UI.Foundation
using Microsoft.EnterpriseManagement.UI.DataModel;      //Contains IDataItem
//Requires Microsoft.EnterpriseManagement.ServiceManager.Application.Common
using Microsoft.EnterpriseManagement.ServiceManager.Application.Common; //Contains ConsoleContextHelper



namespace Microsoft.Demo.ServiceRequest
{
    class CostCenterTaskHandler : ConsoleCommand
    {
        public override void ExecuteCommand(IList<NavigationModelNodeBase> nodes, NavigationModelNodeTask task, ICollection<string> parameters)
        {
            //*** IMPORTANT NOTE: The IManagementGroupSession is not a part of the publicly document/supported official SDK and is subject to change in a future release.
            IManagementGroupSession session = (IManagementGroupSession)FrameworkServices.GetService<IManagementGroupSession>();
            EnterpriseManagementGroup emg = session.ManagementGroup;

            if (parameters.Contains("Edit"))
            {
                //There will only ever be one item because we are going to limit this task to single select
                foreach (NavigationModelNodeBase node in nodes)
                {
                    //*** IMPORTANT NOTE: The ConsoleContextHelper class is not a part of the publicly document/supported official SDK and is subject to change in a future release.
                    ConsoleContextHelper.Instance.PopoutForm(node);
                }

            }
            else if (parameters.Contains("Delete"))
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete the selected Cost Centers?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                    return;

                //Create an IncrementalDiscoveryData "bucket" for capturing all the deletes that will be processed at the same time
                IncrementalDiscoveryData idd = new IncrementalDiscoveryData();

                foreach (NavigationModelNodeBase node in nodes)
                {
                    EnterpriseManagementObject emoCostCenter = emg.EntityObjects.GetObject<EnterpriseManagementObject>(new Guid(node["$Id$"].ToString()), ObjectQueryOptions.Default);
                    idd.Remove(emoCostCenter);
                }

                idd.Commit(emg);
                RequestViewRefresh();
            }
        }
    }
}
