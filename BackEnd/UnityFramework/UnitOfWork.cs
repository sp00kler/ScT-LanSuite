using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Models;
using Dal;
using Repository;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;



namespace UnityFramework
{
    public class UnitOfWork : IDisposable
    {
        ApplicationDbContext context = new ApplicationDbContext();
        #region RepositoryClasses
        private GenericRepository<Clan> ClanRepository;
        private GenericRepository<ApplicationUser> UserRepository;
        private GenericRepository<ApplicationRole> RoleRepository;
        private GenericRepository<IdentityUserLogin> LoginRepository;
        private GenericRepository<Compo> CompoRepository;
        private GenericRepository<Team> TeamRepository;
        private GenericRepository<Edition> EditionRepository;
        private GenericRepository<Registration> RegistrationRepository;
        private GenericRepository<Page> PageRepository;
        private GenericRepository<News> NewsRepository;
        private GenericRepository<Comments> CommentsRepository;
        private GenericRepository<ClanInvitation> ClanInvitationRepository;
        private GenericRepository<ClanSeating> ClanSeatingRepository;
        private GenericRepository<Seating> SeatingRepository;
        private GenericRepository<Settings> SettingsRepository;



        public GenericRepository<Settings> settingsRepository
        {
            get
            {
                if (this.SettingsRepository == null)
                    this.SettingsRepository = new GenericRepository<Settings>(context);
                return SettingsRepository;
            }
        }
        public GenericRepository<Seating> seatingRepository
        {
            get
            {
                if (this.SeatingRepository == null)
                    this.SeatingRepository = new GenericRepository<Seating>(context);
                return SeatingRepository;
            }
        }

        public GenericRepository<ClanSeating> clanSeatingRepository
        {
            get
            {
                if (this.ClanSeatingRepository == null)
                    this.ClanSeatingRepository = new GenericRepository<ClanSeating>(context);
                return ClanSeatingRepository;
            }
        }

        public GenericRepository<Clan> clanRepository
        {
            get
            {
                if (this.ClanRepository == null)
                    this.ClanRepository = new GenericRepository<Clan>(context);
                return ClanRepository;
            }
        }

        public GenericRepository<ApplicationUser> userRepository
        {
            get
            {
                if (this.UserRepository == null)
                    this.UserRepository = new GenericRepository<ApplicationUser>(context);
                return UserRepository;
            }
        }

        public GenericRepository<ApplicationRole> roleRepository
        {
            get
            {
                if (this.RoleRepository == null)
                    this.RoleRepository = new GenericRepository<ApplicationRole>(context);
                return RoleRepository;
            }
        }

        public GenericRepository<IdentityUserLogin> loginRepository
        { 
            get
            {
                if (this.LoginRepository == null)
                    this.LoginRepository = new GenericRepository<IdentityUserLogin>(context);
                return LoginRepository;
            }
        }

        public GenericRepository<Compo> compoRepository
        {
            get
            {
                if (this.CompoRepository == null)
                    this.CompoRepository = new GenericRepository<Compo>(context);
                return CompoRepository;
            }
        }

        public GenericRepository<Team> teamRepository
        {
            get
            {
                if (this.TeamRepository == null)
                    this.TeamRepository = new GenericRepository<Team>(context);
                return TeamRepository;
            }
        }

        public GenericRepository<Edition> editionRepository
        {
            get
            {
                if (this.EditionRepository == null)
                    this.EditionRepository = new GenericRepository<Edition>(context);
                return EditionRepository;
            }
        }

        public GenericRepository<Registration> registrationRepository
        {
            get
            {
                if (this.RegistrationRepository == null)
                    this.RegistrationRepository = new GenericRepository<Registration>(context);
                return RegistrationRepository;
            }
        }

        public GenericRepository<Page> pageRepository
        {
            get
            {
                if (this.PageRepository == null)
                    this.PageRepository = new GenericRepository<Page>(context);
                return PageRepository;
            }
        }

        public GenericRepository<News> newsRepository
        {
            get
            {
                if (this.NewsRepository == null)
                    this.NewsRepository = new GenericRepository<News>(context);
                return NewsRepository;
            }
        }
        public GenericRepository<Comments> commentsRepository
        {
            get
            {
                if (this.CommentsRepository == null)
                    this.CommentsRepository = new GenericRepository<Comments>(context);
                return CommentsRepository;
            }
        }
        public GenericRepository<ClanInvitation> clanInvitationRepository
        {
            get
            {
                if (this.ClanInvitationRepository == null)
                    this.ClanInvitationRepository = new GenericRepository<ClanInvitation>(context);
                return ClanInvitationRepository;
                 
            }
        }
        #endregion
        #region NO CHANGE

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
