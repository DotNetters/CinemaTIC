using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Cinematic.Domain.Contracts
{
    /// <summary>
    /// Data context. 
    /// Provides facilities for querying and working with entity data as objects.
    /// </summary>
    public interface IDataContext : IDisposable
    {
        #region Operaciones del patrón Unit Of Work

        /// <summary>
        /// Commit changes tracked in context to data warehouse
        /// </summary>
        void SaveChanges(); 

        #endregion

        #region Operaciones del patrón Repository

        /// <summary>
        /// Add a new entity instance to the context
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        void Add<T>(T entity) where T : class, IBusinessEntity;

        /// <summary>
        /// Mark for delete the entity instance associated with the identifier
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="id">Entity instance identifier</param>
        void Remove<T>(object id) where T : class, IBusinessEntity;

        /// <summary>
        /// Mark for delete the entity instance
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="entity">Entity instance</param>
        void Remove<T>(T entity) where T : class, IBusinessEntity;

        /// <summary>
        /// Actualiza los datos de una entidad en el contexto
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="entity">Instancia de la entidad</param>
        void Update<T>(T entity) where T : class, IBusinessEntity;

        /// <summary>
        /// Uses the entity identifier to attempt to find an entity tracked by the context. 
        /// If the entity is not in the context then a query will be executed and evaluated against the data 
        /// in the data source, and null is returned if the entity is not found in the context 
        /// or in the data source. Note that the Find also returns entities that have been added to the 
        /// context but have not yet been saved to the database.
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="id">Entity instance identifier</param>
        /// <returns>The entity found, or null.</returns>
        T Find<T>(object id) where T : class, IBusinessEntity;

        /// <summary>
        /// Obtiene las entidades identificados en la lista recibida como parámetro
        /// </summary>
        /// <typeparam name="T">Tipo de la entidad</typeparam>
        /// <param name="ids">Lista de identificadores de los elementos a buscar</param>
        /// <returns>Lista de elementos encontrados</returns>
        T[] Get<T>(params object[] ids) where T : class, IBusinessEntity; 

        #endregion

        #region Colecciones de entidades Root

        /// <summary>
        /// Conjunto de <see cref="Seat">butacas</see> gestionadas por el sistema
        /// </summary>
        IEnumerable<Seat> Seats { get; }

        /// <summary>
        /// Conjunto de <see cref="Ticket">tickets</see> gestionados por el sistema
        /// </summary>
        IEnumerable<Ticket> Tickets { get; }

        /// <summary>
        /// Conjunto de <see cref="Session">sesiones</see> gestionadas por el sistema
        /// </summary>
        IEnumerable<Session> Sessions { get; }

        #endregion
    }
}
