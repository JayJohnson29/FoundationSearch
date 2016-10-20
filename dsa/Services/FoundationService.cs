using System.Collections.Generic;
using System.Linq;
using dsa.Models;
using System.Data.SqlClient;
using System.Data;
using TIMSS;
using TIMSS.Client.Implementation.Security.Authentication;
using TIMSS.SqlObjects;
using System.Text;
using System;

namespace dsa.Services
{
    public class FoundationService
    {
        public class CouncilStatus
        {
            public string Member { get; set; }
            public string Accredited { get; set; }
            public string Operator { get; set; }
        }

        public List<Models.FoundationModel> GetFoundations(CouncilFoundationModel model)
        {
            try
            {
                PersonifyInitializer.Initialize(".\\", new ImmediateSeatInformationProvider());
                IStoredProcedureRequest storedProcedureRequest = new StoredProcedureRequest("USR_COF_CONGRESSIONAL_DISTRICT_SP");

                IStoredProcedureParameterItems invocationParameters = storedProcedureRequest.InvocationParameters;

                invocationParameters.Add(new StoredProcedureParameterItem("inpSTATE", model.State ?? string.Empty, ParameterDirection.Input));
                invocationParameters.Add(new StoredProcedureParameterItem("inpCongressional", model.District, ParameterDirection.Input));

                CouncilStatus status = null;
                switch (model.CouncilStatus)
                {
                    case "0":
                        invocationParameters.Add(new StoredProcedureParameterItem("inpOperator", "OR", ParameterDirection.Input));
                        break;
                    case "1":
                        status = new CouncilStatus { Accredited = "No", Member = "Yes", Operator = "AND" };
                        break;
                    case "2":
                        status = new CouncilStatus { Accredited = "Yes", Member = "No", Operator = "AND" };
                        break;
                    case "3":
                        status = new CouncilStatus { Accredited = "Yes", Member = "Yes", Operator = "AND" };
                        break;
                    case "4":
                        status = new CouncilStatus { Accredited = "Yes", Member = "Yes", Operator = "OR" };
                        break;
                    default:
                        invocationParameters.Add(new StoredProcedureParameterItem("inpOperator", "OR", ParameterDirection.Input));
                        break;
                }

                if (status != null)
                {
                    invocationParameters.Add(new StoredProcedureParameterItem("inpMEMFLAG", status.Member, ParameterDirection.Input));
                    invocationParameters.Add(new StoredProcedureParameterItem("inpAccredited", status.Accredited, ParameterDirection.Input));
                    invocationParameters.Add(new StoredProcedureParameterItem("inpOperator", status.Operator, ParameterDirection.Input));
                }

                IQueryRequest request = new QueryRequest(storedProcedureRequest);
                IQueryResult queryResult = Global.App.Execute(request);


                if (!queryResult.Success || queryResult.DataSet == null || queryResult.DataSet.Tables.Count == 0)
                {

                    var stringBuilder = new StringBuilder();
                    int arg_53_0 = 0;
                    int num = queryResult.ValidationMessages.Count - 1;
                    for (int i = arg_53_0; i <= num; i++)
                    {
                        stringBuilder.Append(queryResult.ValidationMessages[i]);
                        stringBuilder.Append(Environment.NewLine);
                    }
                    var error = stringBuilder.ToString();

                    return new List<FoundationModel>();
                }
                //         0       1          2          3              4          5              6            7        8          9      10    11           12       13 
                /* 'Select ORG_ID, ORG_NAME, MEMSTATUS, ORG_COF_STATUS, ORG_CLASS, CONGRESSIONAL, ADDRESS_1, ADDRESS_2, ADDRESS_3, CITY, STATE, POSTAL_CODE, COUNTRY, ACCREDITED '     */

                var foundations = new List<FoundationModel>();

                if (queryResult.DataSet.Tables[0].Rows.Count > 0)
                {
                    var dt = queryResult.DataSet.Tables[0];

                    for (int i = 0;i < dt.Rows.Count; i++)
                    {
                        var s = dt.Rows[i][0].ToString();
                        var id = 0;
                        Int32.TryParse(s, out id);

                        foundations.Add(new FoundationModel
                        {
                            Id = id,
                            OrgName = dt.Rows[i][1].ToString(),
                            CouncilStatus = dt.Rows[i][3].ToString(),
                            OrgClass = dt.Rows[i][4].ToString(),
                            District = dt.Rows[i][5].ToString(),                           
                            Address1 = dt.Rows[i][6].ToString(),
                            Address2 = dt.Rows[i][7].ToString(),
                            City = dt.Rows[i][9].ToString(),
                            State = dt.Rows[i][10].ToString(),
                            CFNSB = dt.Rows[i][13].ToString(),
                        });
                    }
                }

                return foundations;
            }
            catch (Exception ex)
            {
                return new List<FoundationModel>();
            }
        }

        public List<KeyValuePair<string, string>> GetAllStates()
        {
            try
            {
                PersonifyInitializer.Initialize(".\\", new ImmediateSeatInformationProvider());
                IStoredProcedureRequest storedProcedureRequest = new StoredProcedureRequest("USR_COF_CONGRESSIONAL_DISTRICT_STATES_SP");

                IQueryRequest request = new QueryRequest(storedProcedureRequest);
                IQueryResult queryResult = Global.App.Execute(request);

                var states = new List<KeyValuePair<string, string>>();

                if (!queryResult.Success || queryResult.DataSet == null || queryResult.DataSet.Tables.Count == 0)
                {

                    var stringBuilder = new StringBuilder();
                    int arg_53_0 = 0;
                    int num = queryResult.ValidationMessages.Count - 1;
                    for (int i = arg_53_0; i <= num; i++)
                    {
                        stringBuilder.Append(queryResult.ValidationMessages[i]);
                        stringBuilder.Append(Environment.NewLine);
                    }
                    var error = stringBuilder.ToString();

                    return states;
                }


                if (queryResult.DataSet.Tables[0].Rows.Count > 0)
                {
                    var dt = queryResult.DataSet.Tables[0];

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        states.Add(new KeyValuePair<string, string>(dt.Rows[i][0].ToString(), dt.Rows[i][0].ToString()));
                    }
                }

                return states;
            }
            catch (Exception ex)
            {
                return new List<KeyValuePair<string, string>>();
            }

        }

    }
}
