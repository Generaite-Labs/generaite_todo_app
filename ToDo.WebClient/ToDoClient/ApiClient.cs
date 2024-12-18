// <auto-generated/>
#pragma warning disable CS0618
using Microsoft.Kiota.Abstractions.Extensions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Serialization.Form;
using Microsoft.Kiota.Serialization.Json;
using Microsoft.Kiota.Serialization.Multipart;
using Microsoft.Kiota.Serialization.Text;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using ToDo.WebClient.ToDoClient.Api;
using ToDo.WebClient.ToDoClient.ConfirmEmail;
using ToDo.WebClient.ToDoClient.ForgotPassword;
using ToDo.WebClient.ToDoClient.Login;
using ToDo.WebClient.ToDoClient.Logout;
using ToDo.WebClient.ToDoClient.Manage;
using ToDo.WebClient.ToDoClient.Refresh;
using ToDo.WebClient.ToDoClient.Register;
using ToDo.WebClient.ToDoClient.ResendConfirmationEmail;
using ToDo.WebClient.ToDoClient.ResetPassword;
using ToDo.WebClient.ToDoClient.Roles;
namespace ToDo.WebClient.ToDoClient
{
    /// <summary>
    /// The main entry point of the SDK, exposes the configuration and the fluent API.
    /// </summary>
    [global::System.CodeDom.Compiler.GeneratedCode("Kiota", "1.0.0")]
    public partial class ApiClient : BaseRequestBuilder
    {
        /// <summary>The api property</summary>
        public global::ToDo.WebClient.ToDoClient.Api.ApiRequestBuilder Api
        {
            get => new global::ToDo.WebClient.ToDoClient.Api.ApiRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The confirmEmail property</summary>
        public global::ToDo.WebClient.ToDoClient.ConfirmEmail.ConfirmEmailRequestBuilder ConfirmEmail
        {
            get => new global::ToDo.WebClient.ToDoClient.ConfirmEmail.ConfirmEmailRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The forgotPassword property</summary>
        public global::ToDo.WebClient.ToDoClient.ForgotPassword.ForgotPasswordRequestBuilder ForgotPassword
        {
            get => new global::ToDo.WebClient.ToDoClient.ForgotPassword.ForgotPasswordRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The login property</summary>
        public global::ToDo.WebClient.ToDoClient.Login.LoginRequestBuilder Login
        {
            get => new global::ToDo.WebClient.ToDoClient.Login.LoginRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The logout property</summary>
        public global::ToDo.WebClient.ToDoClient.Logout.LogoutRequestBuilder Logout
        {
            get => new global::ToDo.WebClient.ToDoClient.Logout.LogoutRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The manage property</summary>
        public global::ToDo.WebClient.ToDoClient.Manage.ManageRequestBuilder Manage
        {
            get => new global::ToDo.WebClient.ToDoClient.Manage.ManageRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The refresh property</summary>
        public global::ToDo.WebClient.ToDoClient.Refresh.RefreshRequestBuilder Refresh
        {
            get => new global::ToDo.WebClient.ToDoClient.Refresh.RefreshRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The register property</summary>
        public global::ToDo.WebClient.ToDoClient.Register.RegisterRequestBuilder Register
        {
            get => new global::ToDo.WebClient.ToDoClient.Register.RegisterRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The resendConfirmationEmail property</summary>
        public global::ToDo.WebClient.ToDoClient.ResendConfirmationEmail.ResendConfirmationEmailRequestBuilder ResendConfirmationEmail
        {
            get => new global::ToDo.WebClient.ToDoClient.ResendConfirmationEmail.ResendConfirmationEmailRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The resetPassword property</summary>
        public global::ToDo.WebClient.ToDoClient.ResetPassword.ResetPasswordRequestBuilder ResetPassword
        {
            get => new global::ToDo.WebClient.ToDoClient.ResetPassword.ResetPasswordRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>The roles property</summary>
        public global::ToDo.WebClient.ToDoClient.Roles.RolesRequestBuilder Roles
        {
            get => new global::ToDo.WebClient.ToDoClient.Roles.RolesRequestBuilder(PathParameters, RequestAdapter);
        }
        /// <summary>
        /// Instantiates a new <see cref="global::ToDo.WebClient.ToDoClient.ApiClient"/> and sets the default values.
        /// </summary>
        /// <param name="requestAdapter">The request adapter to use to execute the requests.</param>
        public ApiClient(IRequestAdapter requestAdapter) : base(requestAdapter, "{+baseurl}", new Dictionary<string, object>())
        {
            ApiClientBuilder.RegisterDefaultSerializer<JsonSerializationWriterFactory>();
            ApiClientBuilder.RegisterDefaultSerializer<TextSerializationWriterFactory>();
            ApiClientBuilder.RegisterDefaultSerializer<FormSerializationWriterFactory>();
            ApiClientBuilder.RegisterDefaultSerializer<MultipartSerializationWriterFactory>();
            ApiClientBuilder.RegisterDefaultDeserializer<JsonParseNodeFactory>();
            ApiClientBuilder.RegisterDefaultDeserializer<TextParseNodeFactory>();
            ApiClientBuilder.RegisterDefaultDeserializer<FormParseNodeFactory>();
        }
    }
}
#pragma warning restore CS0618
