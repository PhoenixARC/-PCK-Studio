namespace PckStudio.Core.DLC
{
    public enum DLCPackageContentSerilasationType : int
    {
        Local, //! create local folder with the value of: ('IDS_DISPLAY_NAME') and write all content into it.
        Share  //! zip file if texture or mashup pack, else just the pck file.
    }
}