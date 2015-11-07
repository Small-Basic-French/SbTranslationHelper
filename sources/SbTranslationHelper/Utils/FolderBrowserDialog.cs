using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SbTranslationHelper.Utils
{
    /// <summary>
    /// Prompts the user to select a folder.
    /// </summary>
    public sealed class FolderBrowserDialog : CommonDialog
    {
        #region Fields

        [SecuritySafeCritical]
        private NativeMethods.FolderBrowserOptions _dialogOptions;

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the <see cref="FolderBrowserDialog" /> class.
        /// </summary>
        [SecurityCritical]
        public FolderBrowserDialog()
        {
            Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Resets the properties of a common dialog to their default values.
        /// </summary>
        [SecuritySafeCritical]
        public override void Reset()
        {
            new FileIOPermission(PermissionState.Unrestricted).Demand();

            Initialize();
        }

        /// <summary>
        ///     Displays the folder browser dialog.
        /// </summary>
        /// <param name="hwndOwner">Handle to the window that owns the dialog box.</param>
        /// <returns>
        ///     If the user clicks the OK button of the dialog that is displayed, true is returned; otherwise, false.
        /// </returns>
        [SecuritySafeCritical]
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            bool result = false;

            IntPtr pidlRoot = IntPtr.Zero,
                pszPath = IntPtr.Zero,
                pidlSelected = IntPtr.Zero;

            SelectedPath = string.Empty;

            try
            {
                if (RootType == RootType.SpecialFolder)
                {
                    NativeMethods.SHGetFolderLocation(hwndOwner, (int)RootSpecialFolder, IntPtr.Zero, 0, out pidlRoot);
                }
                else // RootType == Path
                {
                    uint iAttribute;
                    NativeMethods.SHParseDisplayName(RootPath, IntPtr.Zero, out pidlRoot, 0, out iAttribute);
                }

                var browseInfo = new NativeMethods.BROWSEINFO
                {
                    HwndOwner = hwndOwner,
                    Root = pidlRoot,
                    DisplayName = new string(' ', 256),
                    Title = Title,
                    Flags = (uint)_dialogOptions,
                    LParam = 0,
                    Callback = HookProc
                };

                // Show dialog
                pidlSelected = NativeMethods.SHBrowseForFolder(ref browseInfo);

                if (pidlSelected != IntPtr.Zero)
                {
                    result = true;

                    pszPath = Marshal.AllocHGlobal(260 * Marshal.SystemDefaultCharSize);
                    NativeMethods.SHGetPathFromIDList(pidlSelected, pszPath);

                    SelectedPath = Marshal.PtrToStringAuto(pszPath);
                }
            }
            finally // release all unmanaged resources
            {
                NativeMethods.IMalloc malloc = NativeMethods.GetSHMalloc();

                if (pidlRoot != IntPtr.Zero)
                {
                    malloc.Free(pidlRoot);
                }

                if (pidlSelected != IntPtr.Zero)
                {
                    malloc.Free(pidlSelected);
                }

                if (pszPath != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pszPath);
                }

                Marshal.ReleaseComObject(malloc);
            }

            return result;
        }

        [SecurityCritical]
        private void Initialize()
        {
            RootType = RootType.SpecialFolder;
            RootSpecialFolder = Environment.SpecialFolder.Desktop;
            RootPath = string.Empty;
            Title = string.Empty;

            // default options
            _dialogOptions = NativeMethods.FolderBrowserOptions.BrowseFiles
                             | NativeMethods.FolderBrowserOptions.ShowEditBox
                             | NativeMethods.FolderBrowserOptions.UseNewStyle
                             | NativeMethods.FolderBrowserOptions.BrowseShares
                             | NativeMethods.FolderBrowserOptions.ShowStatusText
                             | NativeMethods.FolderBrowserOptions.ValidateResult;
        }

        private bool GetOption(NativeMethods.FolderBrowserOptions option)
        {
            return ((_dialogOptions & option) != NativeMethods.FolderBrowserOptions.None);
        }

        private void SetOption(NativeMethods.FolderBrowserOptions option, bool value)
        {
            if (value)
            {
                _dialogOptions |= option;
            }
            else
            {
                _dialogOptions &= ~option;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the type of the root.
        /// </summary>
        /// <value>The type of the root.</value>
        public RootType RootType { get; set; }

        /// <summary>
        ///     Gets or sets the root path.
        ///     <remarks>Valid only if RootType is set to Path.</remarks>
        /// </summary>
        /// <value>The root path.</value>
        public string RootPath { get; set; }

        /// <summary>
        ///     Gets or sets the root special folder.
        ///     <remarks>Valid only if RootType is set to SpecialFolder.</remarks>
        /// </summary>
        /// <value>The root special folder.</value>
        public Environment.SpecialFolder RootSpecialFolder { get; set; }

        /// <summary>
        ///     Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string SelectedPath { get; set; }

        /// <summary>
        ///     Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether browsing files is allowed.
        /// </summary>
        /// <value></value>
        public bool BrowseFiles
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.BrowseFiles); }
            [SecurityCritical] set { SetOption(NativeMethods.FolderBrowserOptions.BrowseFiles, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to show an edit box.
        /// </summary>
        /// <value></value>
        public bool ShowEditBox
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.ShowEditBox); }
            [SecurityCritical] set { SetOption(NativeMethods.FolderBrowserOptions.ShowEditBox, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether browsing shares is allowed.
        /// </summary>
        /// <value></value>
        public bool BrowseShares
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.BrowseShares); }
            [SecurityCritical] set { SetOption(NativeMethods.FolderBrowserOptions.BrowseShares, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to show status text.
        /// </summary>
        /// <value></value>
        public bool ShowStatusText
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.ShowStatusText); }
            [SecurityCritical] set { SetOption(NativeMethods.FolderBrowserOptions.ShowStatusText, value); }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether to validate the result.
        /// </summary>
        /// <value></value>
        public bool ValidateResult
        {
            get { return GetOption(NativeMethods.FolderBrowserOptions.ValidateResult); }
            [SecurityCritical] set { SetOption(NativeMethods.FolderBrowserOptions.ValidateResult, value); }
        }

        #endregion
    }

    /// <summary>
    ///     Specifies the type of the root folder in a <see cref="FolderBrowserDialog" />.
    /// </summary>
    public enum RootType
    {
        /// <summary>
        ///     Use <see cref="System.Environment.SpecialFolder" />.
        /// </summary>
        SpecialFolder,

        /// <summary>
        ///     Use a path.
        /// </summary>
        Path
    }

    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct BROWSEINFO
        {
            /// <summary>
            ///     Handle to the owner window for the dialog box.
            /// </summary>
            public IntPtr HwndOwner;

            /// <summary>
            ///     Pointer to an item identifier list (PIDL) specifying the
            ///     location of the root folder from which to start browsing.
            /// </summary>
            public IntPtr Root;

            /// <summary>
            ///     Address of a buffer to receive the display name of the
            ///     folder selected by the user.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string DisplayName;

            /// <summary>
            ///     Address of a null-terminated string that is displayed
            ///     above the tree view control in the dialog box.
            /// </summary>
            [MarshalAs(UnmanagedType.LPStr)]
            public string Title;

            /// <summary>
            ///     Flags specifying the options for the dialog box.
            /// </summary>
            public uint Flags;

            /// <summary>
            ///     Address of an application-defined function that the
            ///     dialog box calls when an event occurs.
            /// </summary>
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc Callback;

            /// <summary>
            ///     Application-defined value that the dialog box passes to
            ///     the callback function
            /// </summary>
            public int LParam;

            /// <summary>
            ///     Variable to receive the image associated with the selected folder.
            /// </summary>
            public int Image;
        }

        [Flags]
        public enum FolderBrowserOptions
        {
            /// <summary>
            ///     None.
            /// </summary>
            None = 0,

            /// <summary>
            ///     For finding a folder to start document searching
            /// </summary>
            FolderOnly = 0x0001,

            /// <summary>
            ///     For starting the Find Computer
            /// </summary>
            FindComputer = 0x0002,

            /// <summary>
            ///     Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and
            ///     one line if this flag is set.  Passing the message
            ///     BFFM_SETSTATUSTEXTA to the hwnd can set the rest of the text.
            ///     This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
            ///     all three lines of text.
            /// </summary>
            ShowStatusText = 0x0004,
            ReturnAncestors = 0x0008,

            /// <summary>
            ///     Add an editbox to the dialog
            /// </summary>
            ShowEditBox = 0x0010,

            /// <summary>
            ///     insist on valid result (or CANCEL)
            /// </summary>
            ValidateResult = 0x0020,

            /// <summary>
            ///     Use the new dialog layout with the ability to resize
            ///     Caller needs to call OleInitialize() before using this API
            /// </summary>
            UseNewStyle = 0x0040,
            UseNewStyleWithEditBox = (UseNewStyle | ShowEditBox),

            /// <summary>
            ///     Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
            /// </summary>
            AllowUrls = 0x0080,

            /// <summary>
            ///     Add a UA hint to the dialog, in place of the edit box. May not be
            ///     combined with BIF_EDITBOX.
            /// </summary>
            ShowUsageHint = 0x0100,

            /// <summary>
            ///     Do not add the "New Folder" button to the dialog.  Only applicable
            ///     with BIF_NEWDIALOGSTYLE.
            /// </summary>
            HideNewFolderButton = 0x0200,

            /// <summary>
            ///     don't traverse target as shortcut
            /// </summary>
            GetShortcuts = 0x0400,

            /// <summary>
            ///     Browsing for Computers.
            /// </summary>
            BrowseComputers = 0x1000,

            /// <summary>
            ///     Browsing for Printers.
            /// </summary>
            BrowsePrinters = 0x2000,

            /// <summary>
            ///     Browsing for Everything
            /// </summary>
            BrowseFiles = 0x4000,

            /// <summary>
            ///     sharable resources displayed (remote shares, requires BIF_USENEWUI)
            /// </summary>
            BrowseShares = 0x8000
        }

        #region Malloc

        [ComImport, SuppressUnmanagedCodeSecurity, Guid("00000002-0000-0000-c000-000000000046"),
         InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMalloc
        {
            [PreserveSig]
            IntPtr Alloc(int cb);

            [PreserveSig]
            IntPtr Realloc(IntPtr pv, int cb);

            [PreserveSig]
            void Free(IntPtr pv);

            [PreserveSig]
            int GetSize(IntPtr pv);

            [PreserveSig]
            int DidAlloc(IntPtr pv);

            [PreserveSig]
            void HeapMinimize();
        }

        [SecurityCritical]
        public static IMalloc GetSHMalloc()
        {
            IMalloc[] ppMalloc = new IMalloc[1];
            SHGetMalloc(ppMalloc);
            return ppMalloc[0];
        }

        [SecurityCritical, DllImport("shell32")]
        private static extern int SHGetMalloc([Out, MarshalAs(UnmanagedType.LPArray)] IMalloc[] ppMalloc);

        #endregion

        #region Folders

        [SecurityCritical, DllImport("shell32")]
        public static extern int SHGetFolderLocation(IntPtr hwndOwner, Int32 nFolder, IntPtr hToken, uint dwReserved,
            out IntPtr ppidl);

        [SecurityCritical, DllImport("shell32")]
        public static extern int SHParseDisplayName([MarshalAs(UnmanagedType.LPWStr)] string pszName, IntPtr pbc,
            out IntPtr ppidl, uint sfgaoIn, out uint psfgaoOut);

        [SecurityCritical, DllImport("shell32")]
        public static extern IntPtr SHBrowseForFolder(ref BROWSEINFO lbpi);

        [SecurityCritical, DllImport("shell32", CharSet = CharSet.Auto)]
        public static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

        #endregion

        //#region Helpers

        //public static int IntPtrToInt32(IntPtr intPtr)
        //{
        //    return (int)intPtr.ToInt64();
        //}

        public delegate IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        //#endregion
    }

}
