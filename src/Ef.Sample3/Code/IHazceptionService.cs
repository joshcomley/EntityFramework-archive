using System.Linq;
using WebApplication2.Entities;

namespace WebApplication2
{
    public interface IHazceptionService
    {
        IQueryable<ClientType> ClientTypes { get; }
        IQueryable<ApplicationUser> Users { get; }
        IQueryable<Client> Clients { get; }
        IQueryable<Video> Videos { get; }
        IQueryable<Exam> Exams { get; }
        IQueryable<ExamResult> ExamResults { get; }
        IQueryable<ExamCandidateResult> ExamCandidateResults { get; }
        IQueryable<ExamCandidate> ExamCandidates { get; }
        IQueryable<Hazard> Hazards { get; }
        //IQueryable<ApplicationRole> Roles { get; }
    }
}