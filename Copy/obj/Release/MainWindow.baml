   M S B A M L   `   `   `  ���� ?  ;Copy, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:CopyCopy      Copy.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     $localclr-namespace:Copy       � Title$  CopyFilesToUSB��$	��380��$	��440��$	��380��$	��440��.���  � RowDefinitions �� $��7* ��� $��6* �.+�� $��0�� � ColumnDefinitions �� $
��109* ��� $
��auto ��� $
��186* �.+�� $��0�� �� $	��10* ��� $	��40* �.+�� $��0��.+��� $��	30,20,0,0q�$
��USB:��$��Bottom=�$��0q�� $��1��.+��� -    ��Name$ DevicesListBox��$��
30,0,10,10q����� *DevicesUsb_list+ �� Mode$ TwoWay=� �� SelectedItem �� *SelectedLetter+$ TwoWay=����� .R���  �� Orientation$ 
Horizontal=�.+���  ��Name$ DeviceCheckBox�� �� 	ClickMode$ Press=�$�� ��$��0q�	 d� 	IsChecked	 �� *	IsChecked+$ TwoWay=��� ���� *Name+� $��2�� �� $	��10* ��� $	��40* �.+�� $��0��.+��� $��	10,20,0,0q�$��Bottom=�$��FILES:��$��0q�� $��1��.+��� -   $ FilesListBox��$��
10,0,30,10q����� *
Files_list+$ TwoWay=�� $��1��.+��� -   $��	CopyFiles��$��
25,91,25,0q�$	��Top=�$��30���� -   $��EjectAll��$��25,136,25,0q�$	��Top=�$��30��{�.  �� -   $��SearchFiles��$��
25,45,25,0q�$	��Top=�$��30��� $��1�� �� $	��14* ��� $	��67* �.+�� $��0��.+��� $��	30,10,0,0q�$��Logger:��$��Bottom=�$��0q�� $��1��.+��� -   
 ��Name$
 	LoggerBox��$��
30,0,30,10q�