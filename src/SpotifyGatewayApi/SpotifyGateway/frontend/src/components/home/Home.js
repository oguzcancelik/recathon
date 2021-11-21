import React, {Component} from 'react';
import {connect} from "react-redux";
import UserPlaylists from "../playlists/UserPlaylists";
import Login from "./Login";

class Home extends Component {
    render() {
        return this.props.auth.isLoggedIn
            ? <UserPlaylists/>
            : <Login/>;
    }
}

const mapStateToProps = state => {
    return {
        auth: state.auth
    };
};

export default connect(mapStateToProps)(Home);
