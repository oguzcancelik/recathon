import React from 'react';
import {BrowserRouter, Redirect, Route, Switch} from "react-router-dom";
import Home from "./components/home/Home";
import Navbar from "./components/navbar/Navbar";
import Recommend from "./components/recommendation/Recommend";
import History from "./components/recommendation/History";
import {defaultBackgroundColor} from "./infrastructure/constants/color-constants";
import BrowsePlaylists from "./components/playlists/BrowsePlaylists";

const App = () => {
    return (
        <BrowserRouter>
            <div className="App" style={{backgroundColor: defaultBackgroundColor}}>
                <Navbar/>
                <Switch>
                    <Route exact path='/' component={Home}/>
                    <Route exact path='/browse' component={BrowsePlaylists}/>
                    <Route exact path='/recommend' component={Recommend}/>
                    <Route exact path='/history' component={History}/>
                    <Route>
                        <Redirect to="/"/>
                    </Route>
                </Switch>
            </div>
        </BrowserRouter>
    );
}

export default App;
