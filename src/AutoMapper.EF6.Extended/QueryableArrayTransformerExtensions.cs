using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler;
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoMapper.EF6.Extended
{
  public static class QueryableArrayTransformerExtensions
  {
    /// <summary>
    ///   Projects queryable results to Array asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the results
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains an aary of Transformed results
    /// </returns>
    public static Task<TDestination[]> ProjectToArrayTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToArrayTransformAsync<TDestination>(queryable, config, null);
    }

    /// <summary>
    ///   Projects queryable results to Array asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the results
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains an aary of Transformed results
    /// </returns>
    public static Task<TDestination[]> ProjectToArrayTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToArrayTransformAsync<TDestination>(queryable, config, CancellationToken.None, parameters);
    }

    /// <summary>
    ///   Projects queryable results to Array asynchronously, invoking Transform of each
    ///   <see cref="IPostProjectionTransformer" /> item in the results
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
    ///   The task result contains an aary of Transformed results
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    public static async Task<TDestination[]> ProjectToArrayTransformAsync<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, CancellationToken cancellationToken, object parameters = null)
      where TDestination : IPostProjectionTransformer
    {
      var array =
        await queryable.ProjectTo<TDestination>(config, parameters).DecompileAsync().ToArrayAsync(cancellationToken);

      cancellationToken.ThrowIfCancellationRequested();
      foreach (var item in array.Cast<IPostProjectionTransformer>())
      {
        item.Transform();
      }

      return array;
    }

    /// <summary>
    ///   Projects queryable results to Array, invoking Transform of each <see cref="IPostProjectionTransformer" /> item in the
    ///   results
    /// </summary>
    /// <typeparam name="TDestination">The Destination type.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The <see cref="IConfigurationProvider">configuration provider</see>.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>Array of Transformed results</returns>
    public static TDestination[] ProjectToArrayTransform<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters = null)
      where TDestination : IPostProjectionTransformer
    {
      var array = queryable.ProjectTo<TDestination>(config, parameters).Decompile().ToArray();

      foreach (var item in array.Cast<IPostProjectionTransformer>())
      {
        item.Transform();
      }

      return array;
    }
  }
}