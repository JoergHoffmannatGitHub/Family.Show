/*
 * Exports repositories from the People collection to a Html based report.
 * 
 * The report is based on the same style as the Person report.
 * 
 * All the html containers are in the html document structures region and 
 * are written to the XHtml 1.0 Transitional Standard and CSS 2 standard.
 * For further information see the http://www.w3.org/
 * 
*/

using System;
using System.Globalization;
using System.IO;

namespace FamilyShowLib;

public class RepositoriesExport
{
    #region fields

    private StreamWriter _tw;

    #endregion

    /// <summary>
    /// Export all the data from the Repository collection to the specified html file.
    /// </summary>
    public void ExportRepositories(string fileName, string familyFileName, RepositoryCollection repository)
    {

        _tw = new StreamWriter(fileName);
        //write the necessary html code for a html document
        _tw.WriteLine(Header());
        _tw.WriteLine(CSS());
        _tw.WriteLine(CSSprinting(4));

        _tw.WriteLine("</head><body>");
        _tw.WriteLine("<h2>" + Properties.Resources.FamilyShow + "</h2>");

        _tw.WriteLine(
            "<i>" + Properties.Resources.SummaryOfRepositoriesForFile + " " + familyFileName + "</i><br/><br/>");
        //Write the table headers
        _tw.WriteLine(NormalRepositoryColumns());

        foreach (Repository r in repository)
        {
            _tw.WriteLine(
                "<tr><td><a name=\"" +
                r.Id + "\"></a>" +
                r.Id + "</td><td>" +
                r.RepositoryName + "</td><td>" +
                r.RepositoryAddress + "</td></tr>");
        }

        _tw.WriteLine(Footer());
        _tw.Close();
    }

    #region data output methods

    /// <summary>
    /// Write the column headers for repoisitories
    /// </summary>
    private static string NormalRepositoryColumns()
    {
        return "<table id=\"reositorytable\" border=\"1\" rules=\"all\" frame=\"box\">\n" +
        "<thead>\n" +
        "<tr>\n" +
        "<th width=\"10%\">" + Properties.Resources.Repository + "</th>\n" +
        "<th width=\"15%\">" + Properties.Resources.Name + "</th>\n" +
        "<th width=\"15%\">" + Properties.Resources.Address + "</th>\n" +
        "</tr>\n" +
        "</thead>";

    }

    #endregion

    #region html document structure methods

    private static string Header()
    {
        return "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
                "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" " +
                "\"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">\n" +
                "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\" lang=\"en\">\n" +
                "<head>\n" +
                "<title>" + Properties.Resources.FamilyShow + "</title>";
    }

    /// <summary>
    /// Write the CSS information
    /// </summary>
    private static string CSS()
    {
        return "<style type=\"text/css\">\n" +

                "body { background-color: white; font-family: Calibri, Arial, " +
                "sans-serif; font-size: 12px; line-height: 1.2; padding: 1em; color: #2E2E2E; }\n" +

                "table { border: 0.5px gray solid; width: 100%; empty-cells: show; }\n" +
                "th, td { border: 0.5px gray solid; padding: 0.5em; vertical-align: top; }\n" +
                "td { text-align: left; }\n" +
                "th { background-color: #F0F8FF; }";
    }

    /// <summary>
    /// Write the CSS printing information
    /// </summary>
    private static string CSSprinting(int i)
    {
        string printstyle = "@media print {\n" +
                            "table { border-width: 0px; }\n" +
                            "tr { page-break-inside: avoid; }\n" +
                            "tr >";

        for (int j = 1; j <= i; j++)
        {
            if (i != j)
            {
                printstyle += "*+";
            }
            else
            {
                printstyle += "*";
            }
        }

        printstyle += "{display: none; }\n" +
                        "}\n" +
                        "</style>";

        return printstyle;
    }

    /// <summary>
    /// Write the Footer information
    /// </summary>
    private static string Footer()
    {
        //write the software version and the date and time to the file
        Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        string versionlabel =
            string.Format(CultureInfo.CurrentCulture, "{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        string date = DateTime.Now.ToString();
        return "</table><br/><p><i>" +
            Properties.Resources.GeneratedByFamilyShow + " " +
            versionlabel + " " +
            Properties.Resources.On + " " + date + "</i></p></body></html>";
    }

    #endregion
}
