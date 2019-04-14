using System;
using System.Net.Http;

namespace RestBuilder.Service
{
	//Class comments for Rest
	public partial class Rest
	{
		#region GET Methods

		/// <summary>
		/// Get the specified uri.
		/// </summary>
		/// <returns>The get.</returns>
		/// <param name="uri">URI</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		public static IRestBuilder<TResult, TResult> Get<TResult>(Uri uri) where TResult : class
		{
			return Send<TResult, TResult>(HttpMethod.Get, uri);
		}

		/// <summary>
		/// Get the specified uri.
		/// </summary>
		/// <returns>The get.</returns>
		/// <param name="uri">URI string.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		public static IRestBuilder<TResult, TResult> Get<TResult>(String uri) where TResult : class
		{
			return Send<TResult, TResult>(HttpMethod.Get, uri);
		}

		/// <summary>
		/// Get the specified uri.
		/// </summary>
		/// <returns>The get.</returns>
		/// <param name="uri">URI.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Get<TResult, TSend>(Uri uri) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(HttpMethod.Get, uri);
		}

		/// <summary>
		/// Get the specified uri.
		/// </summary>
		/// <returns>The get.</returns>
		/// <param name="uri">URI string.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Get<TResult, TSend>(String uri) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(HttpMethod.Get, uri);
		}

		#endregion


		#region PUT Methods

		/// <summary>
		/// Put the specified uri and sendObject.
		/// </summary>
		/// <returns>The put.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		public static IRestBuilder<TResult, TResult> Put<TResult>(Uri uri, TResult sendObject) where TResult : class
		{
			return Send<TResult, TResult>(HttpMethod.Put, uri, sendObject);
		}

		/// <summary>
		/// Put the specified uri and sendObject.
		/// </summary>
		/// <returns>The put.</returns>
		/// <param name="uri">URI string.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		public static IRestBuilder<TResult, TResult> Put<TResult>(String uri, TResult sendObject) where TResult : class
		{
			return Send<TResult, TResult>(HttpMethod.Put, uri, sendObject);
		}

		/// <summary>
		/// Put the specified uri and sendObject.
		/// </summary>
		/// <returns>The put.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Put<TResult, TSend>(Uri uri, TSend sendObject) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(HttpMethod.Put, uri, sendObject);
		}

		/// <summary>
		/// Put the specified uri and sendObject.
		/// </summary>
		/// <returns>The put.</returns>
		/// <param name="uri">URI string.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Put<TResult, TSend>(String uri, TSend sendObject) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(HttpMethod.Post, uri, sendObject);
		}


        #endregion


        #region POST Methods

        /// <summary>
        /// Post the specified uri without a sendObject.
        /// </summary>
        /// <returns>The post.</returns>
        /// <param name="uri">URI.</param>
        /// <typeparam name="TResult">The type returned from the REST call</typeparam>
        public static IRestBuilder<TResult, string> Post<TResult>(Uri uri) where TResult : class
        {
            return Send<TResult, string>(HttpMethod.Post, uri);
        }

        /// <summary>
        /// Post the specified uri without a sendObject.
        /// </summary>
        /// <returns>The post.</returns>
        /// <param name="uri">URI.</param>
        /// <typeparam name="TResult">The type returned from the REST call</typeparam>
        public static IRestBuilder<TResult, string> Post<TResult>(string uri) where TResult : class
        {
            return Send<TResult, string>(HttpMethod.Post, uri);
        }

        /// <summary>
        /// Post the specified uri and sendObject.
        /// </summary>
        /// <returns>The post.</returns>
        /// <param name="uri">URI.</param>
        /// <param name="sendObject">Send object.</param>
        /// <typeparam name="TResult">The type returned from the REST call</typeparam>
        public static IRestBuilder<TResult, TResult> Post<TResult>(Uri uri, TResult sendObject) where TResult : class
		{
			return Send<TResult, TResult>(HttpMethod.Post, uri, sendObject);
		}

		/// <summary>
		/// Post the specified uri and sendObject.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="uri">URI string.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		public static IRestBuilder<TResult, TResult> Post<TResult>(String uri, TResult sendObject) where TResult : class
		{
			return Send<TResult, TResult>(HttpMethod.Post, uri, sendObject);
		}

		/// <summary>
		/// Post the specified uri and sendObject.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Post<TResult, TSend>(Uri uri, TSend sendObject) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(HttpMethod.Post, uri, sendObject);
		}

		/// <summary>
		/// Post the specified uri and sendObject.
		/// </summary>
		/// <returns>The post.</returns>
		/// <param name="uri">URI string.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Post<TResult, TSend>(String uri, TSend sendObject) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(HttpMethod.Post, uri, sendObject);
		}


		#endregion


		#region DELETE Methods

		/// <summary>
		/// Delete the specified uri.
		/// </summary>
		/// <returns>The delete.</returns>
		/// <param name="uri">URI.</param>
		public static IRestBuilder<String, String> Delete(Uri uri)
		{
			return Send<String, String>(HttpMethod.Delete, uri);
		}

		/// <summary>
		/// Delete the specified uri.
		/// </summary>
		/// <returns>The delete.</returns>
		/// <param name="uri">URI string.</param>
		public static IRestBuilder<String, String> Delete(String uri)
		{
			return Send<String, String>(HttpMethod.Delete, uri);
		}

		#endregion


		#region Patch Methods

		/// <summary>
		/// Patch the specified uri and sendObject.
		/// </summary>
		/// <returns>The Patch.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		public static IRestBuilder<TResult, TResult> Patch<TResult>(Uri uri, TResult sendObject) where TResult : class
		{
			return Send<TResult, TResult>(new HttpMethod("PATCH"), uri, sendObject);
		}

		/// <summary>
		/// Patch the specified uri and sendObject.
		/// </summary>
		/// <returns>The Patch.</returns>
		/// <param name="uri">URI string.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		public static IRestBuilder<TResult, TResult> Patch<TResult>(String uri, TResult sendObject) where TResult : class
		{
			return Send<TResult, TResult>(new HttpMethod("PATCH"), uri, sendObject);
		}

		/// <summary>
		/// Patch the specified uri and sendObject.
		/// </summary>
		/// <returns>The Patch.</returns>
		/// <param name="uri">URI.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Patch<TResult, TSend>(Uri uri, TSend sendObject) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(new HttpMethod("PATCH"), uri, sendObject);
		}

		/// <summary>
		/// Patch the specified uri and sendObject.
		/// </summary>
		/// <returns>The Patch.</returns>
		/// <param name="uri">URI string.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The type returned from the REST call</typeparam>
		/// <typeparam name="TSend">The type sent to the REST call</typeparam>
		public static IRestBuilder<TResult, TSend> Patch<TResult, TSend>(String uri, TSend sendObject) where TResult : class where TSend : class
		{
			return Send<TResult, TSend>(new HttpMethod("PATCH"), uri, sendObject);
		}

		#endregion




		#region Send Methods

		/// <summary>
		/// Send the specified method, url and optional sendObject to a service API.
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="method">Method.</param>
		/// <param name="url">URL.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		/// <typeparam name="TSend">The 2nd type parameter.</typeparam>
		public static IRestBuilder<TResult, TSend> Send<TResult, TSend>(HttpMethod method, string url, TSend sendObject = null) where TResult : class where TSend : class
		{
			Uri uri = new Uri(url);

			return Send<TResult, TSend>(method, uri, sendObject);
		}

		/// <summary>
		/// Send the specified method, url and optional sendObject to a service API.
		/// </summary>
		/// <returns>The send.</returns>
		/// <param name="method">Method.</param>
		/// <param name="uri">URI.</param>
		/// <param name="sendObject">Send object.</param>
		/// <typeparam name="TResult">The 1st type parameter.</typeparam>
		/// <typeparam name="TSend">The 2nd type parameter.</typeparam>
		public static IRestBuilder<TResult, TSend> Send<TResult, TSend>(HttpMethod method, Uri uri, TSend sendObject = null) where TResult : class where TSend : class
		{
			var restBuilder = new RestBuilder<TResult, TSend>
			{
				Uri = uri,
				Method = method
			};

			if (sendObject != null)
				restBuilder.SendObject = sendObject;

			return restBuilder;
		}

#endregion

	}
}
