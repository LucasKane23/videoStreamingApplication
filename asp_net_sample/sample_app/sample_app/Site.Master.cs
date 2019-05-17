using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace sample_app
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            data_model.CityDataModel cdm =
                new data_model.CityDataModel(
                    System.Configuration.ConfigurationManager.ConnectionStrings["sample_db_connection"].ToString(),
                    "Npgsql",
                    32
                    );

            var result = cdm.getCitiesListTable();

            main_data_grid.DataSource = result;
            main_data_grid.DataBind();
        }
    }
}