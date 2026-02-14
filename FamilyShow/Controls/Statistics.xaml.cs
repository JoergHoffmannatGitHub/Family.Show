using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

using FamilyShowLib;

using Genealogy.Domain.Interfaces;

namespace FamilyShow;

/// <summary>
/// Interaction logic for Statistics.xaml
/// </summary>
public partial class Statistics : UserControl
{
    public Statistics()
    {
        InitializeComponent();
    }

    #region routed events

    public static readonly RoutedEvent CloseButtonClickEvent = EventManager.RegisterRoutedEvent(
        "CloseButtonClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Statistics));

    // Expose this event for this control's container
    public event RoutedEventHandler CloseButtonClick
    {
        add { AddHandler(CloseButtonClickEvent, value); }
        remove { RemoveHandler(CloseButtonClickEvent, value); }
    }

    #endregion

    #region methods

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(CloseButtonClickEvent));
    }

    #endregion

    #region helper methods

    /// <summary>
    /// Display the current file stats
    /// </summary>
    public void DisplayStats(PeopleCollection people, SourceCollection sources, RepositoryCollection repositories)
    {
        #region fields

        double notes = 0;
        double citations = 0;
        double relationshipCitations = 0;

        PhotoCollection allPhotos = [];
        AttachmentCollection allAttachments = [];

        // Events
        double relationships = 0;
        double marriages = 0;
        double divorces = 0;
        double births = 0;
        double deaths = 0;
        double occupations = 0;
        double cremations = 0;
        double burials = 0;
        double educations = 0;
        double religions = 0;

        double livingFacts = 7;
        // Normally a person either has cremation or burial date and place so this counts
        // a 2 events plus death place and death date events plus the normal 7 events.
        double deceasedFacts = 4 + livingFacts;
        double marriageFacts = 2;
        double divorceFacts = 1;
        double living = 0;
        double deceased = 0;

        int minimumYear = DateTime.Now.Year;
        int maximumYear = DateTime.MinValue.Year;

        int marriageDate = int.MaxValue;
        int divorceDate = int.MaxValue;
        int birthDate = int.MaxValue;
        int deathDate = int.MaxValue;
        int cremationDate = int.MaxValue;
        int burialDate = int.MaxValue;

        // Top names
        string[] maleNames = new string[people.Count];
        string[] femaleNames = new string[people.Count];
        string[] surnames = new string[people.Count];

        // Data quality
        double progress = 0;

        int i = 0;  // Counter

        #endregion

        foreach (Person p in people)
        {

            #region top names

            if (p.Gender == Gender.Male)
            {
                maleNames[i] = p.FirstName.Split()[0];
            }
            else
            {
                femaleNames[i] = p.FirstName.Split()[0];
            }

            surnames[i] = p.LastName;

            #endregion

            #region photos

            foreach (Photo photo in p.Photos)
            {

                bool add = true;

                foreach (Photo existingPhoto in allPhotos)
                {

                    if (string.IsNullOrEmpty(photo.RelativePath))
                    {
                        add = false;
                    }

                    if (existingPhoto.RelativePath == photo.RelativePath)
                    {
                        add = false;
                        break;
                    }

                }
                if (add == true)
                {
                    allPhotos.Add(photo);
                }
            }

            #endregion

            #region attachments

            foreach (Attachment attachment in p.Attachments)
            {

                bool add = true;

                foreach (Attachment existingAttachment in allAttachments)
                {

                    if (string.IsNullOrEmpty(attachment.RelativePath))
                    {
                        add = false;
                    }

                    if (existingAttachment.RelativePath == attachment.RelativePath)
                    {
                        add = false;
                        break;
                    }

                }
                if (add == true)
                {
                    allAttachments.Add(attachment);
                }
            }

            #endregion

            if (p.HasNote)
            {
                notes++;
            }

            #region events and citations

            if ((!string.IsNullOrEmpty(p.ReligionSource)) && (!string.IsNullOrEmpty(p.ReligionCitation)) && (!string.IsNullOrEmpty(p.Religion)))
            {
                citations++;
            }

            if ((!string.IsNullOrEmpty(p.CremationSource)) && (!string.IsNullOrEmpty(p.CremationCitation)) && (!string.IsNullOrEmpty(p.CremationPlace) || !DateWrapper.IsNullOrEmpty(p.CremationDate)))
            {
                citations++;
            }

            if ((!string.IsNullOrEmpty(p.OccupationSource)) && (!string.IsNullOrEmpty(p.OccupationCitation)) && (!string.IsNullOrEmpty(p.Occupation)))
            {
                citations++;
            }

            if ((!string.IsNullOrEmpty(p.EducationSource)) && (!string.IsNullOrEmpty(p.EducationCitation)) && (!string.IsNullOrEmpty(p.Education)))
            {
                citations++;
            }

            if ((!string.IsNullOrEmpty(p.BirthSource)) && (!string.IsNullOrEmpty(p.BirthCitation)) && ((!string.IsNullOrEmpty(p.BirthPlace)) || !DateWrapper.IsNullOrEmpty(p.BirthDate)))
            {
                citations++;
            }

            if ((!string.IsNullOrEmpty(p.DeathSource)) && (!string.IsNullOrEmpty(p.DeathCitation)) && ((!string.IsNullOrEmpty(p.DeathPlace)) || !DateWrapper.IsNullOrEmpty(p.DeathDate)))
            {
                citations++;
            }

            if ((!string.IsNullOrEmpty(p.BurialSource)) && (!string.IsNullOrEmpty(p.BurialCitation)) && ((!string.IsNullOrEmpty(p.BurialPlace)) || !DateWrapper.IsNullOrEmpty(p.BurialDate)))
            {
                citations++;
            }

            foreach (Relationship rel in p.Relationships)
            {

                if (rel.RelationshipType == RelationshipType.Spouse)
                {

                    SpouseRelationship spouseRel = ((SpouseRelationship)rel);

                    marriages++;

                    if (!string.IsNullOrEmpty(spouseRel.MarriageCitation) && !string.IsNullOrEmpty(spouseRel.MarriageSource) && (!string.IsNullOrEmpty(spouseRel.MarriagePlace) || DateWrapper.IsNullOrEmpty(spouseRel.MarriageDate)))
                    {
                        relationshipCitations++;
                    }

                    if (!string.IsNullOrEmpty(spouseRel.MarriagePlace))
                    {
                        progress++;
                    }

                    if (DateWrapper.IsDateExact(spouseRel.MarriageDate, out IDateExact exactMarriageDate))
                    {
                        marriageDate = exactMarriageDate.Year;
                        progress++;
                    }

                    if (spouseRel.SpouseModifier == SpouseModifier.Former)
                    {
                        divorces++;

                        if (DateWrapper.IsDateExact(spouseRel.DivorceDate, out IDateExact exactDivorceDate))
                            if (spouseRel.DivorceDate != null)
                            {
                                divorceDate = exactDivorceDate.Year;
                                progress++;
                            }

                        if (!string.IsNullOrEmpty(spouseRel.DivorceCitation) && !string.IsNullOrEmpty(spouseRel.DivorceSource) && !DateWrapper.IsNullOrEmpty(spouseRel.DivorceDate))
                        {
                            relationshipCitations++;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(p.Religion))
            {
                religions++;
            }

            if (!string.IsNullOrEmpty(p.Education))
            {
                educations++;
            }

            if (!string.IsNullOrEmpty(p.Occupation))
            {
                occupations++;
            }

            if (!DateWrapper.IsNullOrEmpty(p.BurialDate) || !string.IsNullOrEmpty(p.BurialPlace))
            {
                burials++;
            }

            if (!DateWrapper.IsNullOrEmpty(p.CremationDate) || !string.IsNullOrEmpty(p.CremationPlace))
            {
                cremations++;
            }

            if (!DateWrapper.IsNullOrEmpty(p.DeathDate) || !string.IsNullOrEmpty(p.DeathPlace))
            {
                deaths++;
            }

            if (!DateWrapper.IsNullOrEmpty(p.BirthDate) || !string.IsNullOrEmpty(p.BirthPlace))
            {
                births++;
            }

            #endregion

            #region min/max dates

            if (DateWrapper.IsDateExact(p.BirthDate, out IDateExact exactBirthDate))
            {
                birthDate = exactBirthDate.Year;
            }

            if (DateWrapper.IsDateExact(p.DeathDate, out IDateExact exactDeathDate))
            {
                deathDate = exactDeathDate.Year;
            }

            if (DateWrapper.IsDateExact(p.CremationDate, out IDateExact exactCremationDate))
            {
                cremationDate = exactCremationDate.Year;
            }

            if (DateWrapper.IsDateExact(p.BurialDate, out IDateExact exactBurialDate))
            {
                burialDate = exactBurialDate.Year;
            }

            int[] list = [marriageDate, divorceDate, birthDate, deathDate, cremationDate, burialDate];
            int yearmin = list.Min();
            int yearmax = list.Max();

            if (minimumYear > yearmin)
            {
                minimumYear = yearmin;
            }

            if (maximumYear < yearmax && yearmax <= DateTime.Now.Year)
            {
                maximumYear = yearmax;
            }

            #endregion

            relationships += p.Relationships.Count;

            #region progress

            if (!string.IsNullOrEmpty(p.FirstName))
            {
                progress++;
            }

            if (!string.IsNullOrEmpty(p.LastName))
            {
                progress++;
            }

            if (!string.IsNullOrEmpty(p.BirthPlace))
            {
                progress++;
            }

            if (!DateWrapper.IsNullOrEmpty(p.BirthDate))
            {
                progress++;
            }

            if (!string.IsNullOrEmpty(p.Occupation))
            {
                progress++;
            }

            if (!string.IsNullOrEmpty(p.Education))
            {
                progress++;
            }

            if (!string.IsNullOrEmpty(p.Religion))
            {
                progress++;
            }

            if (!p.IsLiving)
            {
                deceased++;

                // Only add progress for one cremation or burial not both
                if (!DateWrapper.IsNullOrEmpty(p.CremationDate) || !string.IsNullOrEmpty(p.CremationPlace))
                {
                    if (!DateWrapper.IsNullOrEmpty(p.CremationDate))
                    {
                        progress++;
                    }

                    if (!string.IsNullOrEmpty(p.CremationPlace))
                    {
                        progress++;
                    }
                }
                else
                {
                    if (!DateWrapper.IsNullOrEmpty(p.BurialDate))
                    {
                        progress++;
                    }

                    if (!string.IsNullOrEmpty(p.BurialPlace))
                    {
                        progress++;
                    }
                }

                if (!DateWrapper.IsNullOrEmpty(p.DeathDate))
                {
                    progress++;
                }

                if (!string.IsNullOrEmpty(p.DeathPlace))
                {
                    progress++;
                }
            }
            else
            {
                living++;
            }

            #endregion

            i++;

        }

        // Will have double counted as marriage/divorce/relationships is always between 2 people
        marriages /= 2;
        divorces /= 2;
        relationships /= 2;
        relationshipCitations /= 2;
        citations += relationshipCitations;

        // Media
        // Media data
        double photos = allPhotos.Count;
        double attachments = allAttachments.Count;
        double sourcesCount = sources.Count;
        double repositoriesCount = repositories.Count;

        Photos.Text = Properties.Resources.Photos + ": " + photos;
        Notes.Text = Properties.Resources.Notes + ": " + notes;
        Attachments.Text = Properties.Resources.Attachments + ": " + attachments;
        Citations.Text = Properties.Resources.Citations + ": " + citations;
        Sources.Text = Properties.Resources.Sources + ": " + sourcesCount;
        Repositories.Text = Properties.Resources.Repositories + ": " + repositoriesCount;

        // Relationship and event data
        double totalEvents = births + deaths + marriages + divorces + cremations + burials + educations + occupations + religions;
        Marriages.Text = Properties.Resources.Marriages + ": " + marriages;
        Divorces.Text = Properties.Resources.Divorces + ": " + divorces;

        MinYear.Text = Properties.Resources.EarliestKnownEvent + ": " + minimumYear;

        if (maximumYear == DateTime.MinValue.Year)
        {
            MaxYear.Text = Properties.Resources.LatestKnownEvent + ": " + DateTime.Now.Year;
        }
        else
        {
            MaxYear.Text = Properties.Resources.LatestKnownEvent + ": " + maximumYear;
        }

        if (totalEvents == 0)
        {
            MinYear.Text = Properties.Resources.EarliestKnownEvent + ": ";
            MaxYear.Text = Properties.Resources.LatestKnownEvent + ": ";
        }

        TotalFactsEvents.Text = Properties.Resources.FactsEvents + ": " + totalEvents;
        Relationships.Text = Properties.Resources.Relationships + ": " + relationships;

        // Top 3 names
        string[,] mostCommonNameMale3 = Top3(maleNames);
        string[,] mostCommonNameFemale3 = Top3(femaleNames);
        string[,] mostCommonSurname3 = Top3(surnames);

        FemaleNames.Text =
            Properties.Resources.TopGirlsNames + ": \n" +
            "1. " + mostCommonNameFemale3[0, 0] + " " + mostCommonNameFemale3[0, 1] +
            " 2. " + mostCommonNameFemale3[1, 0] + " " + mostCommonNameFemale3[1, 1] +
            " 3. " + mostCommonNameFemale3[2, 0] + " " + mostCommonNameFemale3[2, 1];
        MaleNames.Text =
            Properties.Resources.TopBoysNames + ": \n" +
            "1. " + mostCommonNameMale3[0, 0] + " " + mostCommonNameMale3[0, 1] +
            " 2. " + mostCommonNameMale3[1, 0] + " " + mostCommonNameMale3[1, 1] +
            " 3. " + mostCommonNameMale3[2, 0] + " " + mostCommonNameMale3[2, 1];
        Surnames.Text =
            Properties.Resources.TopSurnames + ": \n" +
            "1. " + mostCommonSurname3[0, 0] + " " + mostCommonSurname3[0, 1] +
            " 2. " + mostCommonSurname3[1, 0] + " " + mostCommonSurname3[1, 1] +
            " 3. " + mostCommonSurname3[2, 0] + " " + mostCommonSurname3[2, 1];

        #region data quality

        // Data quality is a % measuring the number of sourced events converted to a 5 * rating

        star1.Visibility = Visibility.Collapsed;
        star2.Visibility = Visibility.Collapsed;
        star3.Visibility = Visibility.Collapsed;
        star4.Visibility = Visibility.Collapsed;
        star5.Visibility = Visibility.Collapsed;

        if (totalEvents > 0)
        {
            double dataQuality = citations / totalEvents;
            string tooltip = Math.Round(dataQuality * 100, 2) + "%";

            star1.ToolTip = tooltip;
            star2.ToolTip = tooltip;
            star3.ToolTip = tooltip;
            star4.ToolTip = tooltip;
            star5.ToolTip = tooltip;

            if (dataQuality >= 0)
            {
                star1.Visibility = Visibility.Visible;
            }

            if (dataQuality >= 0.2)
            {
                star2.Visibility = Visibility.Visible;
            }

            if (dataQuality >= 0.4)
            {
                star3.Visibility = Visibility.Visible;
            }

            if (dataQuality >= 0.6)
            {
                star4.Visibility = Visibility.Visible;
            }

            if (dataQuality >= 0.8)
            {
                star5.Visibility = Visibility.Visible;
            }
        }

        #endregion

        #region progress

        // Progress is a measure of the completness of a family tree
        // When a person's fields are completed the completeness score increases
        // (ignores suffix as most people won't have one)

        double totalProgress = progress /
            ((living * livingFacts) +
            (deceased * deceasedFacts) +
            (marriages * marriageFacts) +
            (divorces * divorceFacts));

        if (totalProgress > 100)
        {
            FileProgressBar.Value = 100;
            FileProgressText.Text = "100%";
        }
        else
        {
            FileProgressBar.Value = totalProgress * 100;
            FileProgressText.Text = Math.Round(totalProgress * 100, 2) + "%";
        }

        #endregion

        #region size

        //Data size breakdown of file sizes
        string appLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            App.ApplicationFolderName);

        appLocation = Path.Combine(appLocation, App.AppDataFolderName);

        // Absolute path to the photos folder
        string photoLocation = Path.Combine(appLocation, Photo.PhotosFolderName);
        string noteLocation = Path.Combine(appLocation, Story.StoriesFolderName);
        string attachmentLocation = Path.Combine(appLocation, Attachment.AttachmentsFolderName);
        string xmlLocation = Path.Combine(appLocation, "content.xml");
        string currentLocation = App.FamilyCollection.FullyQualifiedFilename;

        long photoSize = 0;
        long attachmentSize = 0;
        long noteSize = 0;
        long xmlSize = 0;
        long currentSize = 0;

        if (Directory.Exists(photoLocation))
        {
            photoSize = FileSize(Directory.GetFiles(photoLocation, "*.*"));
        }

        if (Directory.Exists(noteLocation))
        {
            noteSize = FileSize(Directory.GetFiles(noteLocation, "*.*"));
        }

        if (Directory.Exists(attachmentLocation))
        {
            attachmentSize = FileSize(Directory.GetFiles(attachmentLocation, "*.*"));
        }

        if (File.Exists(xmlLocation))
        {
            xmlSize = (new FileInfo(xmlLocation).Length) / 1024;  //convert to Kb
        }

        if (File.Exists(currentLocation))
        {
            currentSize = (new FileInfo(currentLocation).Length) / 1024;  //convert to Kb
        }

        if (currentSize > 0)
        {
            DataSize.Text =
                Properties.Resources.DataSize + ": " +
                currentSize + " KB - (" +
                Properties.Resources.Photos + " " +
                photoSize + " KB, " +
                Properties.Resources.Notes + " " +
                noteSize + " KB, " +
                Properties.Resources.Attachments + " " +
                attachmentSize + " KB, " +
                Properties.Resources.Xml + " " +
                xmlSize + " KB)";
        }
        else
        {
            DataSize.Text = Properties.Resources.DataSize + ": ";
        }

        #endregion

        #region charts

        //Gender bar chart

        ListCollectionView histogramView = CreateView("Gender", "Gender");
        GenderDistributionControl.View = histogramView;
        GenderDistributionControl.CategoryLabels.Clear();
        GenderDistributionControl.CategoryLabels.Add(Gender.Male, Properties.Resources.Male);
        GenderDistributionControl.CategoryLabels.Add(Gender.Female, Properties.Resources.Female);

        //Living bar chart

        ListCollectionView histogramView2 = CreateView("IsLiving", "IsLiving");
        LivingDistributionControl.View = histogramView2;

        LivingDistributionControl.CategoryLabels.Clear();

        LivingDistributionControl.CategoryLabels.Add(false, Properties.Resources.Deceased);
        LivingDistributionControl.CategoryLabels.Add(true, Properties.Resources.Living);

        //Age group bar chart

        ListCollectionView histogramView4 = CreateView("AgeGroup", "AgeGroup");
        AgeDistributionControl.View = histogramView4;

        AgeDistributionControl.CategoryLabels.Clear();

        AgeDistributionControl.CategoryLabels.Add(AgeGroup.Youth, Properties.Resources.AgeGroupYouth);
        AgeDistributionControl.CategoryLabels.Add(AgeGroup.Unknown, Properties.Resources.AgeGroupUnknown);
        AgeDistributionControl.CategoryLabels.Add(AgeGroup.Senior, Properties.Resources.AgeGroupSenior);
        AgeDistributionControl.CategoryLabels.Add(AgeGroup.MiddleAge, Properties.Resources.AgeGroupMiddleAge);
        AgeDistributionControl.CategoryLabels.Add(AgeGroup.Adult, Properties.Resources.AgeGroupAdult);

        #endregion

        // Ensure the controls are refreshed
        AgeDistributionControl.Refresh();
        SharedBirthdays.Refresh();
        LivingDistributionControl.Refresh();
        GenderDistributionControl.Refresh();

    }

    private void PrintButton_Click(object sender, EventArgs e)
    {
        PrintDialog dlg = new();

        //Animated progress bar does not render well on print
        FileProgressBar.Visibility = Visibility.Collapsed;

        if (dlg.ShowDialog().GetValueOrDefault())
        {
            Print(dlg, StatisticsPanel);
        }

        //Animated progress bar does not render well on print
        FileProgressBar.Visibility = Visibility.Visible;
    }

    private static void Print(PrintDialog pd, Border border)
    {
        // Make a Grid to hold the contents.
        Grid pageArea = new();

        double padding = 20;
        double titlepadding = 25;

        pageArea.Height = border.Height;

        VisualBrush diagramFill = new(border);

        // Titles
        TextBlock titles = new();
        if (!string.IsNullOrEmpty(App.FamilyCollection.FullyQualifiedFilename))
        {
            titles.Text =
                Properties.Resources.StatisticsReportFor + " " +
                Path.GetFileName(App.FamilyCollection.FullyQualifiedFilename);
        }
        else
        {
            titles.Text = Properties.Resources.StatisticsReport;
        }

        // Data
        System.Windows.Shapes.Rectangle diagram = new()
        {
            Margin = new Thickness(0, titlepadding, 0, 0),
            Fill = diagramFill,
            Stroke = Brushes.Black,
            StrokeThickness = 0.5
        };

        // Page Area
        pageArea.Margin = new Thickness(padding);
        pageArea.Children.Add(titles);
        pageArea.Children.Add(diagram);

        // Arrange
        Rect container = new(0, 0, border.ActualWidth, border.ActualHeight + titlepadding);
        pageArea.Arrange(container);

        string title = Path.GetFileName(
            App.FamilyCollection.FullyQualifiedFilename) + " " + Properties.Resources.StatisticsReport;

        // Print
        pd.PrintVisual(pageArea, title);
    }

    private static ListCollectionView CreateView(string group, string sort)
    {
        ListCollectionView view = new(App.Family);

        // Apply sorting
        if (!string.IsNullOrEmpty(sort))
        {
            view.SortDescriptions.Add(new SortDescription(sort, ListSortDirection.Ascending));
        }

        // Group the collection into tags. The tag cloud will be based on the group Name and ItemCount
        PropertyGroupDescription groupDescription = new();
        if (!string.IsNullOrEmpty(group))
        {
            groupDescription.PropertyName = group;
        }

        view.GroupDescriptions.Add(groupDescription);

        return view;
    }

    /// <summary>
    /// Returns the 3 most common strings in an array of strings.
    /// On tie break, appends *
    /// </summary>
    /// <param name="Strings"></param>
    /// <returns></returns>
    private static string[,] Top3(string[] Strings)
    {

        string[,] Top3 = new string[3, 2];

        int y1 = 0;  //1st highest count
        int y2 = 0;  //2nd highest count
        int y3 = 0;  //3rd highest count

        bool firstTie = false;
        bool secondTie = false;
        bool thirdTie = false;

        foreach (string n in Strings)
        {
            int z = 0; // Count of n in Strings

            if (n != Properties.Resources.Unknown && !string.IsNullOrEmpty(n))
            {
                foreach (string s in Strings)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (n.Trim() == s.Trim())
                        {
                            z++;
                        }
                    }
                }

                if (z >= y1)
                {
                    if (z > y1)
                    {
                        firstTie = false; // Unique
                    }

                    if (z == y1 && !string.IsNullOrEmpty(Top3[0, 0]) && Top3[0, 0] != n.Trim())
                    {
                        firstTie = true;  // Tied
                    }

                    y1 = z;

                    Top3[0, 0] = n.Trim();
                    Top3[0, 1] = "(" + y1.ToString() + ")";

                }
                else if (z >= y2)
                {

                    if (z > y2)
                    {
                        secondTie = false; // Unique
                    }

                    if (z == y2 && !string.IsNullOrEmpty(Top3[1, 0]) && Top3[1, 0] != n.Trim())
                    {
                        secondTie = true;  // Tied
                    }

                    y2 = z;

                    Top3[1, 0] = n.Trim();
                    Top3[1, 1] = "(" + y2.ToString() + ")";

                }
                else if (z >= y3)
                {

                    if (z > y3)
                    {
                        thirdTie = false; // Unique
                    }

                    if (z == y3 && !string.IsNullOrEmpty(Top3[2, 0]) && Top3[2, 0] != n.Trim())
                    {
                        thirdTie = true;  // Tied
                    }

                    y3 = z;

                    Top3[2, 0] = n.Trim();
                    Top3[2, 1] = "(" + y3.ToString() + ")";

                }

            }



        }

        // Append * if tied
        if (firstTie)
        {
            Top3[0, 0] += "*";
        }

        if (secondTie)
        {
            Top3[1, 0] += "*";
        }

        if (thirdTie)
        {
            Top3[2, 0] += "*";
        }

        return Top3;
    }

    /// <summary>
    /// Returns the total file size in Kb of an array of file names
    /// </summary>
    /// <param name="a"></param>
    /// <returns></returns>
    private static long FileSize(string[] a)
    {
        long b = 0;
        foreach (string name in a)
        {
            FileInfo info = new(name);
            b += info.Length;
        }

        b /= 1024; //Kb

        return b;
    }

    #endregion
}
