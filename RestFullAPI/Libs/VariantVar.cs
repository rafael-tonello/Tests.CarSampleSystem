using System;

namespace RestFullAPI
{
	public class VariantVar
	{
		private string _value;
		public VariantVar (){}
		public VariantVar (object value)
		{
			
		}

		public string AsString {
			get { return _value; }
			set { _value = value; }	
		}

		public int AsInt {
			get { return int.Parse(_value); }
			set { _value = value.ToString(); }	
		}

		public double AsDouble {
			get { return double.Parse(_value); }
			set { _value = value.ToString(); }	
		}

		public char AsChar {
			get { return _value[0]; }
			set { _value = value + ""; }	
		}

		public bool AsBool {
			get { return _value.ToLower() != "false" || _value != "0"; }
			set { _value = value.ToString(); }	
		}


	}
}

