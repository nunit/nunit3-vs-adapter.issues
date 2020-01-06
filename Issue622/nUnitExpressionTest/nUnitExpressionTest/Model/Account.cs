
using System;
using System.Collections.Generic;

namespace nUnitExpressionTest.Model
{
    public class Account : IEquatable<Account>
    {
        public string ID { get; set; }

        /// <summary>
        /// Parses the <see cref="Value"/> as a <see cref="Guid"/>. If the ID is not a valid Guid, then null is returned.
        /// </summary>
        public Guid? IDAsGuid
        {
            get { try { return Guid.ParseExact(ID, "N"); } catch { return null; } }
            set => ID = value?.ToString("N") ?? "";
        }
        public string Name { get; set; }
        public Account ParentAccount { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as Account);
        }

        public bool Equals(Account account)
        {
            return account != null &&
                   ID == account.ID;
        }

        public override int GetHashCode()
        {
            return 1213502048 + EqualityComparer<string>.Default.GetHashCode(ID);
        }

        public static Account operator +(Account rl) => rl;

        public static bool operator ==(Account left, Account right)
        {
            return EqualityComparer<Account>.Default.Equals(left, right);
        }

        public static bool operator !=(Account left, Account right)
        {
            return !(left == right);
        }

        public static implicit operator Guid(Account rl) => rl.IDAsGuid.GetValueOrDefault();

        public static implicit operator Guid?(Account rl) => rl.IDAsGuid;

        public static implicit operator string(Account rl) => rl.ID;
    }
}
