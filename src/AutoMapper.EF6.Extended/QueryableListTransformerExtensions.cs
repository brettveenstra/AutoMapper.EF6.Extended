using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler;
using DelegateDecompiler.EntityFramework;

namespace AutoMapper.EF6.Extended
{
  public static class QueryableListTransformerExtensions
  {
    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    public static Task<List<TDestination>> ProjectToListTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToListTransformAsync<TDestination>(queryable, config, null, CancellationToken.None);
    }

    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    public static Task<List<TDestination>> ProjectToListTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToListTransformAsync<TDestination>(queryable, config, parameters, CancellationToken.None);
    }

    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
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
    public static Task<List<TDestination>> ProjectToListTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, CancellationToken cancellationToken)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToListTransformAsync<TDestination>(queryable, config, null, cancellationToken);
    }

    /// <summary>
    ///   Projects queryable results to List asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains List of Transformed items
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    public static async Task<List<TDestination>> ProjectToListTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters, CancellationToken cancellationToken)
      where TDestination : IPostProjectionTransformer
    {
      var list =
        await queryable.ProjectTo<TDestination>(config, parameters).DecompileAsync().ToListAsync(cancellationToken);

      cancellationToken.ThrowIfCancellationRequested();
      foreach (var item in list.Cast<IPostProjectionTransformer>())
      {
        item.Transform();
      }

      return list;
    }

    /// <summary>
    ///   Projects queryable results to List, invoking Transform of each <see cref="IPostProjectionTransformer" /> item in the
    ///   final list
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>List of Transformed items</returns>
    public static List<TDestination> ProjectToListTransform<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters = null)
      where TDestination : IPostProjectionTransformer
    {
      var list = queryable.ProjectTo<TDestination>(config, parameters).Decompile().ToList();

      foreach (var item in list.Cast<IPostProjectionTransformer>())
      {
        item.Transform();
      }

      return list;
    }
  }
}