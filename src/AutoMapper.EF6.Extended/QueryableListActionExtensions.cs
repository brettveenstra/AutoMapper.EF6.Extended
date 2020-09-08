using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMapper.EF6.Extended
{
  public static class QueryableListActionExtensions
  {
    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking <paramref name="postProjectionAction" /> against each
    ///   item in final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="postProjectionAction">The post projection action to apply to the items in the result set.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    public static Task<List<TDestination>> ProjectToListActionAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, Action<TDestination> postProjectionAction)
    {
      return ProjectToListActionAsync(queryable, config, postProjectionAction, null, CancellationToken.None);
    }

    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking <paramref name="postProjectionAction" /> against each
    ///   item in final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="postProjectionAction">The post projection action to apply to the items in the result set.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    public static Task<List<TDestination>> ProjectToListActionAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, Action<TDestination> postProjectionAction, CancellationToken cancellationToken)
    {
      return ProjectToListActionAsync(queryable, config, postProjectionAction, null, cancellationToken);
    }

    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking <paramref name="postProjectionAction" /> against each
    ///   item in final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="postProjectionAction">The post projection action to apply to the items in the result set.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    public static Task<List<TDestination>> ProjectToListActionAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, Action<TDestination> postProjectionAction, object parameters)
    {
      return ProjectToListActionAsync(queryable, config, postProjectionAction, parameters, CancellationToken.None);
    }

    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking <paramref name="postProjectionAction" /> against each
    ///   item in final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="postProjectionAction">The post projection action to apply to the items in the result set.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    public static async Task<List<TDestination>> ProjectToListActionAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, Action<TDestination> postProjectionAction, object parameters,
      CancellationToken cancellationToken)
    {
      var list =
        await queryable.ProjectTo<TDestination>(config, parameters).DecompileAsync().ToListAsync(cancellationToken);

      if (postProjectionAction == null)
      {
        return list;
      }

      cancellationToken.ThrowIfCancellationRequested();
      list.ForEach(postProjectionAction);

      return list;
    }

    /// <summary>
    ///   Projects queryable results to List, invoking <paramref name="postProjectionAction" /> against each item in final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="postProjectionAction">The post projection action.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   List of Transformed results
    /// </returns>
    public static List<TDestination> ProjectToListAction<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, Action<TDestination> postProjectionAction = null, object parameters = null)
    {
      var list = queryable.ProjectTo<TDestination>(config, parameters).Decompile().ToList();

      if (postProjectionAction == null)
      {
        return list;
      }

      list.ForEach(postProjectionAction);

      return list;
    }
  }
}