using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using WebApplication2.Entities;

namespace WebApplication2
{
    public static class ODataConfigurator
    {
        public static IEdmModel ConfigureHazception(IApplicationBuilder app)
        {
            IAssemblyProvider provider = app.ApplicationServices.GetRequiredService<IAssemblyProvider>();
            var builder = new ODataConventionModelBuilder(provider);
            builder.Namespace = "Hazception";
            builder.EntitySet<ExamResult>(nameof(IHazceptionService.ExamResults));
            builder.EntitySet<ExamCandidate>(nameof(IHazceptionService.ExamCandidates));
            builder.EntitySet<ExamCandidateResult>(nameof(IHazceptionService.ExamCandidateResults));
            builder.EntitySet<ApplicationUser>(nameof(IHazceptionService.Users));
            builder.EntitySet<Hazard>(nameof(IHazceptionService.Hazards));
            builder.EntitySet<Video>(nameof(IHazceptionService.Videos));
            builder.EntitySet<Client>(nameof(IHazceptionService.Clients));
            builder.EntitySet<Exam>(nameof(IHazceptionService.Exams));
            //      builder
            //          .EntitySet<ApplicationUser>(nameof(IHazceptionService.Users) + "2");
            //      builder
            //          .EntityType<ApplicationUser>()
            //          .Collection
            //          .Function(nameof(UsersController.Me))
            //          //.Returns<string>()
            //          .ReturnsFromEntitySet<ApplicationUser>(nameof(IHazceptionService.Users) + "2")
            //          ;
            //return builder.GetEdmModel();
            // OData actions are HTTP POST
            // OData functions are HTTP GET
            builder
                .EntityType<ExamCandidate>()
                .HasKey(ec => ec.Id);

            builder.EntityType<ApplicationUser>()
                //.HasKey(p => p.Id)
                ;
            //.Property(p => p.FullName)
            //.DeclaringType
            //.IgnoredProperties;


            //builder.EntityType<ApplicationUser>()
            //.RemoveAllProperties()
            //.AddProperty(p => p.Client)
            //.AddProperty(p => p.ClientId)
            //.AddProperty(p => p.Email)
            //.AddProperty(p => p.FullName)
            //.AddProperty(p => p.EmailConfirmed)
            //.AddProperty(p => p.UserType)
            //            .AddProperty(p => p.IsLockedOut)
            //;

            builder.EntityType<ExamResult>();

            //builder.ConfigureClientImports();
            //builder.ConfigureUserImports();
            //builder.ConfigureVideoImports();
            //builder.ConfigureExamImports();
            //builder.ConfigureHazardImports();

            //builder.AddODataServerFieldValidation();

            //builder.EntityType<Hazard>().AddOdataServerEntityValidation();
            //builder.EntityType<Video>().AddOdataServerEntityValidation();
            //builder.EntityType<Client>().AddOdataServerEntityValidation();
            //builder.EntityType<Exam>().AddOdataServerEntityValidation();
            //builder.EntityType<ApplicationUser>().AddOdataServerEntityValidation();
            return builder.GetEdmModel();
        }
    }
}