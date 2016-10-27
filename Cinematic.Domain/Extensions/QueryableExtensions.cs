using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cinematic.Extensions
{
    /// <summary>
    /// Extensions methods for <see cref="IQueryable{T}"/>
    /// </summary>
    /// <remarks>
    /// See the reasons in Cazzulino's blog: http://blogs.clariusconsulting.net/kzu/how-to-design-a-unit-testable-domain-model-with-entity-framework-code-first/
    /// One critical thing at this point is the Include. 
    /// This is an extension method over <see cref="IQueryable{T}"/> provided by the DbExtensions class in EF 4.1. So how can we possibly mock it?
    /// First thing first: consumers of our domain context will never have an import of the System.Data.Entity namespace. Ever. They don’t care about IDbSet, Database, DbContext, etc. So the extension method will never be in context for them.
    /// </remarks>
    public static class QueryableExtensions
    {
        public static IIncluder Includer = new NullIncluder();

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <typeparam name="T">The type of the entity being queried.</typeparam>
        /// <typeparam name="TProperty">The type of the navigation property being included.</typeparam>
        /// <param name="source">The source IQueryable on which to call Include.</param>
        /// <param name="path">A lambda expression representing the path to include.</param>
        /// <returns>A new IQueryable of T with the defined query path.</returns>
        public static IQueryable<T> Include<T, TProperty>(this IQueryable<T> source, Expression<Func<T, TProperty>> path)
             where T : class
        {
            return Includer.Include(source, path);
        }

        /// <summary>
        /// Entity includer
        /// </summary>
        public interface IIncluder
        {
            /// <summary>
            /// Specifies the related objects to include in the query results.
            /// </summary>
            /// <typeparam name="T">The type of the entity being queried.</typeparam>
            /// <typeparam name="TProperty">The type of the navigation property being included.</typeparam>
            /// <param name="source">The source IQueryable on which to call Include.</param>
            /// <param name="path">A lambda expression representing the path to include.</param>
            /// <returns>A new IQueryable of T with the defined query path.</returns>
            IQueryable<T> Include<T, TProperty>(IQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class;
        }

        internal class NullIncluder : IIncluder
        {
            public IQueryable<T> Include<T, TProperty>(IQueryable<T> source, Expression<Func<T, TProperty>> path)
                 where T : class
            {
                return source;
            }
        }
    }
}

