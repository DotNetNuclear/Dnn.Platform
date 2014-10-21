﻿#region Copyright
// 
// DotNetNuke® - http://www.dotnetnuke.com
// Copyright (c) 2002-2014
// by DotNetNuke Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Linq;
using DotNetNuke.Entities.Content.Workflow.Repositories;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace DotNetNuke.Entities.Content.Workflow
{
    internal class SystemWorkflowManager : ServiceLocator<ISystemWorkflowManager, SystemWorkflowManager>, ISystemWorkflowManager
    {
        public const string DirectPublishWorkflowKey = "DirectPublish";
        public const string SaveDraftWorkflowKey = "SaveDraft";
        public const string ContentAprovalWorkflowKey = "ContentApproval";

        private readonly IWorkflowRepository _workflowRepository;
        private readonly IWorkflowStateRepository _workflowStateRepository;

        public SystemWorkflowManager()
        {
            _workflowRepository = WorkflowRepository.Instance;
            _workflowStateRepository = WorkflowStateRepository.Instance;
        }

        public void CreateSystemWorkflows(int portalId)
        {
            CreateDirectPublishWorkflow(portalId);
            CreateSaveDraftWorkflow(portalId);
            CreateContentApprovalWorkflow(portalId);
        }

        public ContentWorkflow GetDirectPublishWorkflow(int portalId)
        {
            return _workflowRepository.GetSystemWorkflows(portalId).SingleOrDefault(sw => sw.WorkflowKey == DirectPublishWorkflowKey);
        }

        public ContentWorkflow GetSaveDraftWorkflow(int portalId)
        {
            return _workflowRepository.GetSystemWorkflows(portalId).SingleOrDefault(sw => sw.WorkflowKey == SaveDraftWorkflowKey);
        }

        public ContentWorkflow GetContentApprovalWorkflow(int portalId)
        {
            return _workflowRepository.GetSystemWorkflows(portalId).SingleOrDefault(sw => sw.WorkflowKey == ContentAprovalWorkflowKey);
        }

        public ContentWorkflowState GetDraftStateDefinition(int order)
        {
            var state = GetDefaultWorkflowState(order);
            state.StateName = Localization.GetString("DefaultWorkflowState1.StateName");
            return state;
        }

        public ContentWorkflowState GetPublishedStateDefinition(int order)
        {
            var state = GetDefaultWorkflowState(order);
            state.StateName = Localization.GetString("DefaultWorkflowState3.StateName");
            return state;
        }

        public ContentWorkflowState GetReadyForReviewStateDefinition(int order)
        {
            var state = GetDefaultWorkflowState(order);
            state.StateName = Localization.GetString("DefaultWorkflowState2.StateName");
            return state;
        }

        private ContentWorkflowState GetDefaultWorkflowState(int order)
        {
            return new ContentWorkflowState
            {
                IsSystem = true,
                SendNotification = true,
                SendNotificationToAdministrators = false,
                Order = order
            };
        }

        private void CreateDirectPublishWorkflow(int portalId)
        {
            var workflow = new ContentWorkflow
            {
                WorkflowName = Localization.GetString("DefaultDirectPublishWorkflowName"),
                Description = Localization.GetString("DefaultDirectPublishWorkflowDescription"),
                WorkflowKey = DirectPublishWorkflowKey,
                IsSystem = true,
                PortalID = portalId
            };
            _workflowRepository.AddWorkflow(workflow);
            var publishedState = GetPublishedStateDefinition(1);
            publishedState.WorkflowID = workflow.WorkflowID;
            _workflowStateRepository.AddWorkflowState(publishedState);
        }

        private void CreateSaveDraftWorkflow(int portalId)
        {
            var workflow = new ContentWorkflow
            {
                WorkflowName = Localization.GetString("DefaultSaveDraftWorkflowName"),
                Description = Localization.GetString("DefaultSaveDraftWorkflowDescription"),
                WorkflowKey = SaveDraftWorkflowKey,
                IsSystem = true,
                PortalID = portalId
            };
            _workflowRepository.AddWorkflow(workflow);

            var state = GetDraftStateDefinition(1);
            state.WorkflowID = workflow.WorkflowID;
            _workflowStateRepository.AddWorkflowState(state);
            
            state = GetPublishedStateDefinition(2);
            state.WorkflowID = workflow.WorkflowID;
            _workflowStateRepository.AddWorkflowState(state);
        }

        private void CreateContentApprovalWorkflow(int portalId)
        {
            var workflow = new ContentWorkflow
            {
                WorkflowName = Localization.GetString("DefaultWorkflowName"),
                Description = Localization.GetString("DefaultWorkflowDescription"),
                WorkflowKey = ContentAprovalWorkflowKey,
                IsSystem = true,
                PortalID = portalId
            };
            _workflowRepository.AddWorkflow(workflow);

            var state = GetDraftStateDefinition(1);
            state.WorkflowID = workflow.WorkflowID;
            _workflowStateRepository.AddWorkflowState(state);

            state = GetReadyForReviewStateDefinition(2);
            state.WorkflowID = workflow.WorkflowID;
            _workflowStateRepository.AddWorkflowState(state);

            state = GetPublishedStateDefinition(3);
            state.WorkflowID = workflow.WorkflowID;
            _workflowStateRepository.AddWorkflowState(state);
        }

        protected override Func<ISystemWorkflowManager> GetFactory()
        {
            return () => new SystemWorkflowManager();
        }
    }
}