using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;





namespace BasicPlugin 
{
    public class FollowupPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

           
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            
            if (context.InputParameters.Contains("Target") &&
                context.InputParameters["Target"] is Entity)
            {
                  
                Entity entity = (Entity)context.InputParameters["Target"];

                  
                  
                IOrganizationServiceFactory serviceFactory =
                    (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    Entity followup = new Entity("task");

                    followup["subject"] = "Send e-mail to the new customer.";
                    followup["description"] = "Follow up with the customer. Check if there are any new issues that need resolution.";

                    followup["scheduledstart"] = DateTime.Now.AddDays(7);
                    followup["scheduledend"] = DateTime.Now.AddDays(7);
                    followup["cotegory"] = context.PrimaryEntityName;

                    if (context.OutputParameters.Contains("id"))
                    {
                        Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                        string regardingobjectidType = "account";
                        followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                    }
                    tracingService.Trace("FollowupPlugin: Creating the task activity.");
                    service.Create(followup);

                    Entity dailyIncome = new Entity("Daily Income");
                    Entity invoce = new Entity("Invoce");
                    Entity account = new Entity("Account");

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }

        }
    }
}
