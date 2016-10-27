using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Cinematic.Core;
using Cinematic.Core.Contracts;

namespace Cinematic.DAL
{
    public class CinematicEFDataContext : DbContext, IDataContext
    {
        #region Query expansion

        private class DbIncluder : Cinematic.Extensions.QueryableExtensions.IIncluder
        {
            public IQueryable<T> Include<T, TProperty>(IQueryable<T> source, Expression<Func<T, TProperty>> path)
                where T : class
            {
                return System.Data.Entity.QueryableExtensions.Include(source, path);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor. Buildup the DbIncluder in the QueryableExtensions class
        /// </summary>
        static CinematicEFDataContext()
        {
            Cinematic.Extensions.QueryableExtensions.Includer = new DbIncluder();
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name 
        /// or connection string for the database to which a connection will be made.
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string.</param>
        public CinematicEFDataContext() : base("Cinematic") { } 

        #endregion

        #region Unit of Work pattern operations

        void IDataContext.SaveChanges()
        {
            this.SaveChanges();
        }

        #endregion

        #region Repository pattern operations

        /// <summary>
        /// <see cref="IDataContext.Get"/>
        /// </summary>
        /// <typeparam name="T"><see cref="IDataContext.Get"/></typeparam>
        /// <param name="ids"><see cref="IDataContext.Get"/></param>
        /// <returns><see cref="IDataContext.Get"/></returns>
        public T[] Get<T>(object[] ids) where T : class, IBusinessEntity
        {
            T[] retVal = new T[ids.Count()];

            int i = 0;
            foreach (var id in ids)
            {
                retVal[i] = Find<T>(id);
                i++;
            }
            return retVal;
        }

        /// <summary>
        /// <see cref="Kash.Core.DALContracts.IContext.Add"/>
        /// </summary>
        /// <typeparam name="T"><see cref="Kash.Core.DALContracts.IContext.Add"/></typeparam>
        /// <param name="entity"><see cref="Kash.Core.DALContracts.IContext.Add"/></param>
        public void Add<T>(T entity) where T : class, IBusinessEntity
        {
            this.Set<T>().Add(entity);
        }

        /// <summary>
        /// <see cref="IDataContext.Remove{T}(System.Guid)"/>
        /// </summary>
        /// <typeparam name="T"><see cref="IDataContext.Remove{T}(System.Guid)"/></typeparam>
        /// <param name="id"><see cref="IDataContext.Remove{T}(System.Guid)"/></param>
        void IDataContext.Remove<T>(object id)
        {
            var set = this.Set<T>();
            var entity = set.Find(id);

            if (entity != null)
            {
                set.Remove(entity);
            }
            else
            {
                throw new ApplicationException("No entity has been found with the specified identifier");
            }
        }

        /// <summary>
        /// <see cref="Kash.Core.DALContracts.IContext.Remove{T}(T)"/>
        /// </summary>
        /// <typeparam name="T"><see cref="Kash.Core.DALContracts.IContext.Remove{T}(T)"/></typeparam>
        /// <param name="entity"><see cref="Kash.Core.DALContracts.IContext.Remove{T}(T)"/></param>
        void IDataContext.Remove<T>(T entity)
        {
            var set = this.Set<T>();
            var foundEntity = set.Find(entity.Id);
            if (foundEntity != null)
            {
                set.Remove(foundEntity);
            }
            else
            {
                throw new ApplicationException("Entity has not been found");
            }
        }

        /// <summary>
        /// Actualiza los datos de una entidad en el contexto
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="entity">Instancia de la entidad</param>
        void IDataContext.Update<T>(T entity)
        {
            this.Entry<T>(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// <see cref="Kash.Core.Contracts.DAL.IDataContext.Find{T}"/>
        /// </summary>
        /// <typeparam name="T"><see cref="Kash.Core.Contracts.DAL.IDataContext.Find{T}"/></typeparam>
        /// <param name="id"><see cref="Kash.Core.Contracts.DAL.IDataContext.Find{T}"/></param>
        /// <returns><see cref="Kash.Core.Contracts.DAL.IDataContext.Find{T}"/></returns>
        public T Find<T>(object id) where T : class, IBusinessEntity
        {
            return this.Set<T>().Find(id);
        } 

        #endregion

        #region Operaciones de bajo nivel

        /// <summary>
        /// <see cref="Kash.Core.DALContracts.IContext.SqlQuery"/>
        /// </summary>
        /// <param name="elementType"><see cref="Kash.Core.DALContracts.IContext.SqlQuery"/></param>
        /// <param name="sql"><see cref="Kash.Core.DALContracts.IContext.SqlQuery"/></param>
        /// <param name="parameters"><see cref="Kash.Core.DALContracts.IContext.SqlQuery"/></param>
        /// <returns><see cref="Kash.Core.DALContracts.IContext.SqlQuery"/></returns>
        public IEnumerable SqlQuery(Type elementType, string sql, params object[] parameters)
        {
            return Database.SqlQuery(elementType, sql, parameters);
        }

        /// <summary>
        /// <see cref="Kash.Core.Contracts.DAL.IDataContext.SqlQuery{TElement}"/>
        /// </summary>
        /// <typeparam name="TElement"><see cref="Kash.Core.Contracts.DAL.IDataContext.SqlQuery{TElement}"/></typeparam>
        /// <param name="sql"><see cref="Kash.Core.Contracts.DAL.IDataContext.SqlQuery{TElement}"/></param>
        /// <param name="parameters"><see cref="Kash.Core.Contracts.DAL.IDataContext.SqlQuery{TElement}"/></param>
        /// <returns><see cref="Kash.Core.Contracts.DAL.IDataContext.SqlQuery{TElement}"/></returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return Database.SqlQuery<TElement>(sql, parameters);
        }

        /// <summary>
        /// <see cref="Kash.Core.DALContracts.IContext.ExecuteSqlCommand"/>
        /// </summary>
        /// <param name="sql"><see cref="Kash.Core.DALContracts.IContext.ExecuteSqlCommand"/></param>
        /// <param name="parameters"><see cref="Kash.Core.DALContracts.IContext.ExecuteSqlCommand"/></param>
        /// <returns><see cref="Kash.Core.DALContracts.IContext.ExecuteSqlCommand"/></returns>
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return Database.ExecuteSqlCommand(sql, parameters);
        } 

        #endregion

        public IDbSet<Seat> Seats { get; set; }

        public IDbSet<Ticket> Tickets { get; set; }

        public IDbSet<Session> Sessions { get; set; }

        IEnumerable<Seat> IDataContext.Seats
        {
            get { return this.Seats; }
        }

        IEnumerable<Ticket> IDataContext.Tickets
        {
            get { return this.Tickets; }
        }

        IEnumerable<Session> IDataContext.Sessions
        {
            get { return this.Sessions; }
        }
    }
}
