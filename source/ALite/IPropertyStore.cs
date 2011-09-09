using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite
{
	/// <summary>
	/// Interface defining the basic behaviour of a property store.
	/// </summary>
	/// <typeparam name="DocumentType">The type of document used to store property data.</typeparam>
	public interface IPropertyStore<DocumentType>
	{
		/// <summary>
		/// Gets the property store document.
		/// </summary>
		DocumentType Document { get; }

		/// <summary>
		/// Backs up the current state of the store.
		/// </summary>
		void SetRestorePoint();

		/// <summary>
		/// Sets the specified property to the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="name">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		void SetProperty<T>(string name, T value);

		/// <summary>
		/// Gets the value of the specified property.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="name">The name of the property.</param>
		/// <returns>The value of the property.</returns>
		T GetProperty<T>(string name);

		/// <summary>
		/// Removes the specified property from the store.
		/// </summary>
		/// <param name="name">The name of the property to remove.</param>
		void RemoveProperty(string name);

		/// <summary>
		/// Restores to the last backed up state.
		/// </summary>
		void RevertToRestorePoint();

		/// <summary>
		/// Overwrites the existing data with a copy of the supplied document.
		/// </summary>
		/// <param name="data">The document containing the new data for the store.</param>
		void InjectData(DocumentType data);
	}
}
