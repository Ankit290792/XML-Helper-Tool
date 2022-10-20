using Deloitte.Ink.Transition.Importer.Reports.Models;
using Deloitte.Ink.Common;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Deloitte.Ink.Transition.Importer.Reports;
using Deloitte.Ink.Transition.Importer.Reports.Interface;
using Serilog;
using Serilog.Events;
using Deloitte.Ink.Transition.Importer.Reports.Services;
using Deloitte.Ink.Transition.Importer.Reports.WorkFlow;
using Deloitte.Ink.Transition.Importer.Reports.Common;
using Deloitte.Ink.Transition.Importer.Reports.ExcelReports;
namespace Deloitte.Ink.Transition.Importer.Reports.StartUp
{
    public static class ServiceExtension
    {



        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration Configuration)
        {

            services.AddHostedService<ConsoleHostedService>();
            services.AddTransient<IProcessor, Processor>();
            services.AddScoped<ITransitionCoreServices, TransitionCoreServices>();
            services.AddScoped<IStandardWorkflow, Deloitte.Ink.Transition.Importer.Reports.WorkFlow.StandardWorkflow>();
            services.AddScoped<IManualWorkflow, Deloitte.Ink.Transition.Importer.Reports.WorkFlow.ManualWorkflow>();
            services.AddScoped<IPracticalGuideWorkflow, Deloitte.Ink.Transition.Importer.Reports.WorkFlow.PracticalGuideWorkflow>();
            services.AddTransient<IWorkflowResolver, WorkflowResolver>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<ITransitionContextService, TransitionContextService>();
            services.AddScoped<IPracticalGuideService, PracticalGuideService>();
            services.AddTransient<IManualReportService, ManualReportService>();
            services.AddTransient<IStandardReportService, StandardReportService>();
            services.AddTransient<IElementsReportWorkflow, ElementsWorkflow>();
            services.AddTransient<IHierarchialReports, ImporterHierarchialReports>();
            services.AddTransient<IHierarchyReportWorkflow, HierarchyWorkflow>();
            services.AddTransient<IElementsReport, ElementsReport>();
                    services.AddOptions<LocationConfig>().Bind(Configuration.GetSection("LocationConfig"));
            return services;
        }
    }
}
