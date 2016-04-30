using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using DelegateDecompiler;
using DelegateDecompiler.EntityFramework;

namespace AutoMapper.EF6.Extended
{
  public static class QueryableSingleOrDefaultTransformerExtensions
  {
    /// <summary>
    ///   Asynchronously projects to the only element of a sequence (using the <see cref="IPostProjectionTransformer" />
    ///   behavior), or a default value if the sequence is empty; this method throws an exception if there is more than one
    ///   element in the sequence.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains a single element of the sequence, or <c>default</c> (<typeparamref name="TDestination" /> if
    ///   the sequence contains no elements.
    /// </returns>
    /// <exception cref="System.InvalidOperationException"><paramref name="queryable" /> has more than one element.</exception>
    public static Task<TDestination> ProjectToSingleOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToSingleOrDefaultTransformAsync<TDestination>(queryable, config, null, CancellationToken.None);
    }

    /// <summary>
    ///   Asynchronously projects to the only element of a sequence (using the <see cref="IPostProjectionTransformer" />
    ///   behavior), or a default value if the sequence is empty; this method throws an exception if there is more than one
    ///   element in the sequence.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains a single element of the sequence, or <c>default</c> (<typeparamref name="TDestination" /> if
    ///   the sequence contains no elements.
    /// </returns>
    /// <exception cref="System.InvalidOperationException"><paramref name="queryable" /> has more than one element.</exception>
    public static Task<TDestination> ProjectToSingleOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config, object parameters)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToSingleOrDefaultTransformAsync<TDestination>(queryable, config, parameters, CancellationToken.None);
    }

    /// <summary>
    ///   Asynchronously projects to the only element of a sequence (using the <see cref="IPostProjectionTransformer" />
    ///   behavior), or a default value if the sequence is empty; this method throws an exception if there is more than one
    ///   element in the sequence.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains a single element of the sequence, or <c>default</c> (<typeparamref name="TDestination" /> if
    ///   the sequence contains no elements.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    /// <exception cref="System.InvalidOperationException"><paramref name="queryable" /> has more than one element.</exception>
    public static Task<TDestination> ProjectToSingleOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config, CancellationToken cancellationToken)
      where TDestination : IPostProjectionTransformer
    {
      return ProjectToSingleOrDefaultTransformAsync<TDestination>(queryable, config, null, cancellationToken);
    }

    /// <summary>
    ///   Asynchronously projects to the only element of a sequence (using the <see cref="IPostProjectionTransformer" />
    ///   behavior), or a default value if the sequence is empty; this method throws an exception if there is more than one
    ///   element in the sequence.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <param name="cancellationToken">
    ///   A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the
    ///   task to complete.
    /// </param>
    /// <returns>
    ///   A task that represents the asynchronous operation.
    ///   The task result contains a single element of the sequence, or <c>default</c> (<typeparamref name="TDestination" /> if
    ///   the sequence contains no elements.
    /// </returns>
    /// <exception cref="System.OperationCanceledException">
    ///   Thrown if
    ///   <param name="cancellationToken">cancellationToken</param>
    ///   requests cancellation.
    /// </exception>
    /// <exception cref="System.InvalidOperationException"><paramref name="queryable" /> has more than one element.</exception>
    public static async Task<TDestination> ProjectToSingleOrDefaultTransformAsync<TDestination>(
      this IQueryable queryable, IConfigurationProvider config, object parameters, CancellationToken cancellationToken)
      where TDestination : IPostProjectionTransformer
    {
      var item =
        await
          queryable.ProjectTo<TDestination>(config, parameters).DecompileAsync().SingleOrDefaultAsync(cancellationToken);

      if (item != null)
      {
        cancellationToken.ThrowIfCancellationRequested();

        item.Transform();
      }

      return item;
    }

    /// <summary>
    ///   Projects to the only element of a sequence (using the <see cref="IPostProjectionTransformer" /> behavior), or a
    ///   default value if the sequence is empty; this method throws an exception if there is more than one element in the
    ///   sequence.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="config">The configuration.</param>
    /// <param name="parameters">The values to be used for parameterization.</param>
    /// <returns>
    ///   The single element of the sequence, or <c>default</c> (<typeparamref name="TDestination" /> if the sequence
    ///   contains no elements.
    /// </returns>
    /// <exception cref="System.InvalidOperationException"><paramref name="queryable" /> has more than one element.</exception>
    public static TDestination ProjectToSingleOrDefaultTransform<TDestination>(this IQueryable queryable,
      IConfigurationProvider config, object parameters)
      where TDestination : IPostProjectionTransformer
    {
      var item =
        queryable.ProjectTo<TDestination>(config, parameters).Decompile().SingleOrDefault();

      if (item != null)
      {
        item.Transform();
      }

      return item;
    }
  }
}