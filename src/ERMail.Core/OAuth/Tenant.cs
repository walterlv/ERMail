using System;
using JetBrains.Annotations;

namespace Walterlv.ERMail.OAuth
{
    public class Tenant : IEquatable<Tenant>
    {
        /// <summary>
        /// Allows users with both personal Microsoft accounts and work/school accounts from Azure Active Directory to sign into the application.
        /// </summary>
        public static Tenant Common = new Tenant("common");

        /// <summary>
        /// Allows only users with work/school accounts from Azure Active Directory to sign into the application.
        /// </summary>
        public static Tenant Organizations = new Tenant("organizations");

        /// <summary>
        /// Allows only users with personal Microsoft accounts (MSA) to sign into the application.
        /// </summary>
        public static Tenant Consumers = new Tenant("consumers");

        /// <summary>
        /// Initialize a new instance of <see cref="Tenant"/>.
        /// </summary>
        /// <param name="tenant"></param>
        public Tenant([NotNull] string tenant)
        {
            _value = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        /// <summary>
        /// Gets the tenant value.
        /// </summary>
        private readonly string _value;

        /// <inheritdoc />
        public bool Equals(Tenant other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(_value, other._value);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Tenant) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _value;
        }

        public static implicit operator string(Tenant tenant)
        {
            return tenant._value;
        }

        public static implicit operator Tenant(string tenant)
        {
            return new Tenant(tenant);
        }
    }
}
