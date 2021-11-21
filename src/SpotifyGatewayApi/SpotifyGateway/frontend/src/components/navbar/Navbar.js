import React, {Fragment} from 'react';
import {NavLink} from "react-router-dom";
import '../../css/index.css'
import {connect} from "react-redux";
import {logout} from "../../store/actions/authActions";
import {clearPlaylists} from "../../store/actions/playlistActions";
import {defaultColor, white} from "../../infrastructure/constants/color-constants";

const Navbar = props => (
    <div>
        <nav style={{backgroundColor: defaultColor}}>
            <div className="container" style={{fontFamily: 'Roboto', fontSize: 50}}>
                <NavLink className="brand-logo" style={{color: white}} to="/">Recathlon</NavLink>
                <ul style={{fontFamily: 'Roboto'}} className="right">
                    <li style={{fontFamily: 'Roboto'}}>
                        <NavLink style={{color: white, fontSize: 20}} to="/">
                            {props.auth.isLoggedIn ? "Playlists" : "Home"}
                        </NavLink>
                    </li>

                    {
                        props.auth.isLoggedIn
                            ? (
                                <Fragment>
                                    <li style={{fontFamily: 'Roboto'}}>
                                        <NavLink style={{color: white, fontSize: 20}} to="/browse">Browse</NavLink>
                                    </li>
                                    <li style={{fontFamily: 'Roboto'}}>
                                        <NavLink style={{color: white, fontSize: 20}} to="/history">History</NavLink>
                                    </li>
                                    <li style={{fontFamily: 'Roboto'}}>
                                        <NavLink style={{color: white, fontSize: 20}} onClick={() => {
                                            props.logout();
                                            props.clearPlaylists();
                                        }} to="">Logout</NavLink>
                                    </li>
                                </Fragment>
                            )
                            : null
                    }
                </ul>
            </div>
        </nav>
    </div>
);

const mapStateToProps = (state) => {
    return {
        auth: state.auth
    };
};

const mapDispatchToProps = (dispatch) => {
    return {
        logout: () => dispatch(logout()),
        clearPlaylists: () => dispatch(clearPlaylists())
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(Navbar);

