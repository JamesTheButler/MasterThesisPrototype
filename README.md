# Master Thesis Prototype

*This prototype was developed as the main project for my master thesis with the title **Real-Time Plastic Deformation of Car Bodyworks**.*
<p align="center">
<img src="/media/crash_animation.gif" width="500px">
</p>
The aim of the thesis was to investigate the development of a deformation system for car bodyworks that can achieve real-time performance and is entirely physically simulated. 
The surface mesh of the given vehicle is tetrahedralized via <a href="https://github.com/Yixin-Hu/TetWild">TetWild</a>. The tetrahedralized mesh can be deformed with a 
<a href="https://github.com/JamesTheButler/PlasticDeformationDLL">custom deformation library</a>. The library can be prompted to compute the deformations of a
tetrahedral mesh given a set of colliders and the meshes position in space. Deformations of the tetrahedral mesh are then mapped to the original surface mesh and 
displayed on screen for the user.



This project encompasses a Unity project that prompts the deformation library each frame for the deformation calculations. This makes use of Unity's rendering capabilities, while
using the library for the computationally more complex deformation computations. The deformation library implements simulation based dynamics and is run 
entirely on the CPU. It can compute frames within 10 milliseconds.
