using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimpleDbConnector;
using System.Collections.Generic;
using System.Data;

namespace sample_app.data_model
{
    public class City
    {
        #region data_members

        private string _name;
        private string _country_code;
        private string _district;
        private UInt32 _population;

        #endregion

        #region data_properties

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string CountryCode
        {
            get
            {
                return _country_code;
            }
        }

        public string District
        {
            get
            {
                return _district;
            }
        }

        public UInt32 Population
        {
            get
            {
                return _population;
            }
        }

        #endregion
        public City(string name, string country_code, string district, UInt32 population)
        {
            _name = name;
            _country_code = country_code;
            _district = district;
            _population = population;
        }
    }

    public class CityDataModel : DataModel
    {
        public CityDataModel(String connection_string, String provider_name, int pool_size)
            :
            base(connection_string, provider_name, pool_size)
        { }

        public List<City> getCitiesList()
        {
            throw new NotImplementedException("WIP");
        }

        public DataTable getCitiesListTable()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            return ExecProc("f_cities_list", parameters);
        }
    }
}