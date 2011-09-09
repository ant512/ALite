using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALite
{
	public interface IPropertyStore<DocumentType>
	{
		DocumentType Document { get; }
		void SetRestorePoint();
		void SetProperty<T>(string name, T value);
		T GetProperty<T>(string name);
		void RemoveProperty(string name);
		void RevertToRestorePoint();
		void InjectData(DocumentType data);
	}
}
