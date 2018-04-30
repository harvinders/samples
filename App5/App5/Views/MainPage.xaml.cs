using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using App5.ViewModels;

using Windows.UI.Xaml.Controls;
using Simple;

namespace App5.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var types = new List<Type>();// Assembly.Load(new AssemblyName("Simple")).ExportedTypes.ToList();
            //foreach (var assembly in GetAssemblyList().Result)
            //{
            //    types.AddRange(assembly.ExportedTypes);
            //}
            //var names = Assembly.GetAssembly(typeof(MainPage)).GetReferencedAssemblies();


            List<Assembly> assemblies = new List<Assembly>();

            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync().AsTask().ConfigureAwait(false);

            foreach (var file in files.Where(file => file.FileType == ".dll" ))
            {
                try
                {
                    types.AddRange(Assembly.Load(new AssemblyName(file.DisplayName)).ExportedTypes);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }

            int i = 0;

            foreach (var type in types)
            {
                //if (type is MyClass)
                //{
                //    i = 0;
                //}

                var name = type.Name;
            }
        }

        public static async Task<List<Assembly>> GetAssemblyList()
        {
            List<Assembly> assemblies = new List<Assembly>();

            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            if (files == null)
                return assemblies;

            foreach (var file in files.Where(file => file.FileType == ".dll" || file.FileType == ".exe"))
            {
                try
                {
                    assemblies.Add(Assembly.Load(new AssemblyName(file.DisplayName)));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }

            }

            return assemblies;
        }
        public static IEnumerable<Type> FromApplication(bool skipOnError = true)
        {
            return FromCheckedAssemblies(GetAssembliesApplicationAsync(skipOnError).Result, skipOnError);
        }
        public static IEnumerable<Type> FromCheckedAssemblies(IEnumerable<Assembly> assemblies, bool skipOnError)
        {
            return assemblies
                .SelectMany(a =>
                {
                    IEnumerable<TypeInfo> types;

                    try
                    {
                        types = a.DefinedTypes;
                    }
                    catch (ReflectionTypeLoadException e)
                    {
                        if (!skipOnError)
                        {
                            throw;
                        }

                        types = e.Types.TakeWhile(t => t != null).Select(t => t.GetTypeInfo());
                    }

                    return types.Where(ti => ti.IsClass & !ti.IsAbstract && !ti.IsValueType && ti.IsVisible).Select(ti => ti.AsType());
                });
        }

        private static async Task<IEnumerable<Assembly>> GetAssembliesApplicationAsync(bool skipOnError)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assemblies = new List<Assembly>();

            foreach (var file in await folder.GetFilesAsync().AsTask().ConfigureAwait(false))
            {

                var fileType = Path.GetExtension(file.Name);

                if (fileType == ".dll" || fileType == ".exe")
                {
                    var name = new AssemblyName() { Name = Path.GetFileNameWithoutExtension(file.Name) };
                    Assembly assembly;

                    try
                    {
                        assembly = Assembly.Load(name);
                    }
                    catch (Exception e)
                    {
                        if (!(skipOnError && (e is FileNotFoundException || e is BadImageFormatException)))
                        {
                            throw;
                        }

                        continue;
                    }

                    assemblies.Add(assembly);

                }
            }

            return assemblies;
        }

    }
}
