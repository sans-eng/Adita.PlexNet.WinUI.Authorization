using Microsoft.UI.Xaml;
using Microsoft.Xaml.Interactivity;
using System.Threading;
using System;
using System.Security.Principal;

namespace Adita.PlexNet.WinUI.Authorization.Assists
{
    /// <summary>
    /// Represents an authorization for a <see cref="UIElement"/>.
    /// </summary>
    public sealed class AuthorizeBehavior : Behavior<UIElement>
    {
        #region Dependency properties
        /// <summary>
        /// Identifies the <see cref="Roles"/> property.
        /// </summary>
        public static DependencyProperty RolesProperty =>
            DependencyProperty.Register("Roles", typeof(string), typeof(AuthorizeBehavior), new PropertyMetadata(string.Empty, OnRolesChanged));
        #endregion Dependency properties

        #region Public properties
        /// <summary>
        /// Gets or sets a semicolon delimited list of roles.
        /// </summary>
        public string Roles
        {
            get => (string)GetValue(RolesProperty);
            set => SetValue(RolesProperty, value);
        }
        #endregion Public properties

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            if (Thread.CurrentPrincipal is not Adita.PlexNet.Core.Security.Principals.ApplicationPrincipal applicationPrincipal)
            {
                throw new InvalidOperationException($"Current {nameof(Thread.CurrentPrincipal)} is not {nameof(Core.Security.Principals.ApplicationPrincipal)}");
            }
            applicationPrincipal.IdentityChanged += OnIdentityChanged;
            AuthorizeAction(Roles);
        }
        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (Thread.CurrentPrincipal is not Adita.PlexNet.Core.Security.Principals.ApplicationPrincipal applicationPrincipal)
            {
                throw new InvalidOperationException($"Current {nameof(Thread.CurrentPrincipal)} is not {nameof(Core.Security.Principals.ApplicationPrincipal)}");
            }
            applicationPrincipal.IdentityChanged -= OnIdentityChanged;
        }
        #endregion Protected methods

        #region Dependency property changed event handlers
        private static void OnRolesChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            if (dependencyObject is not AuthorizeBehavior behavior)
            {
                return;
            }

            if (args.NewValue is not string newRoles)
            {
                return;
            }

            behavior.AuthorizeAction(newRoles);
        }
        #endregion Dependency property changed event handlers

        #region Private methods
        private void OnIdentityChanged(object? sender, IIdentity e)
        {
            AuthorizeAction(Roles);
        }
        private void AuthorizeAction(string roles)
        {
            if (roles is null)
            {
                throw new ArgumentNullException(nameof(roles));
            }

            if (AssociatedObject == null)
            {
                return;
            }

            var isAuthorized = IsInRoles(roles);
            SetAuthorization(isAuthorized, AssociatedObject);
        }
        private static bool IsInRoles(string roles)
        {
            if (Thread.CurrentPrincipal?.Identity != null)
            {
                if (string.IsNullOrWhiteSpace(roles) && Thread.CurrentPrincipal.Identity.IsAuthenticated)
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(roles))
                {
                    return false;
                }

                foreach (var role in roles.Split(';'))
                {
                    if (Thread.CurrentPrincipal.IsInRole(role))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        private static void SetAuthorization(bool isAuthorized, UIElement uIElement)
        {
            if (uIElement is null)
            {
                throw new ArgumentNullException(nameof(uIElement));
            }

            if (isAuthorized)
            {
                uIElement.Visibility = Visibility.Visible;
            }
            else
            {
                uIElement.Visibility = Visibility.Collapsed;
            }
        }
        #endregion Private methods
    }
}
